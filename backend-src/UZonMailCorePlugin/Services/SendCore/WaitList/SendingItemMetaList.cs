using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UZonMail.Core.Services.SendCore.WaitList;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Emails;
using UZonMail.DB.SQL.EmailSending;

namespace UZonMail.Core.Services.SendCore.EmailWaitList
{
    /// <summary>
    /// 发件项元数据列表
    /// </summary>
    public class SendingItemMetaList()
    {
        // 为了加速查找，使用字典: key = sendingItemId
        // 包含所有的发件箱
        private readonly ConcurrentDictionary<long, SendItemMeta> _allWaitDic = [];
        // 没有指定发件箱，保存到此处
        private readonly ConcurrentBag<SendItemMeta> _anoymousList = [];

        // 指定发件箱，保存到此处
        private readonly ConcurrentDictionary<long, ConcurrentBag<SendItemMeta>> _outboxDic = [];

        /// <summary>
        /// 处于进行中的项
        /// </summary>
        private readonly ConcurrentDictionary<long, SendItemMeta> _recycleBin = [];

        /// <summary>
        /// 计数器
        /// </summary>
        public SendingItemsCounter Counter { get; } = new();

        /// <summary>
        /// 总数量
        /// </summary>
        public int WaitListCount => _allWaitDic.Count;

        /// <summary>
        /// 回收站中的数量
        /// </summary>
        public int RecycleBinCount => _recycleBin.Count;

        /// <summary>
        /// 待发送的数量
        /// </summary>
        public int ToSendingCount => WaitListCount + RecycleBinCount;

        /// <summary>
        /// 所有的发送项 Id
        /// 待发送与回收站中的项
        /// </summary>
        public IEnumerable<long> SendingItemIds => _allWaitDic.Keys.Concat(_recycleBin.Keys);

        /// <summary>
        /// 添加 SendItemMeta
        /// 若是特定项，则添加到指定发件箱
        /// 否则添加到通用列表
        /// 若不是回收站中的数据，则增加计数
        /// </summary>
        /// <param name="itemMeta"></param>
        /// <param name="fromRecycle">是否来自回收站，回收站的数据不参与统计</param>
        /// <returns></returns>
        public bool Add(SendItemMeta itemMeta, bool fromRecycle = false)
        {
            if (itemMeta == null) return false;

            if (!_allWaitDic.TryAdd(itemMeta.SendingItemId, itemMeta))
                return false;

            if (itemMeta.OutboxId > 0)
            {
                // 特定发件箱
                if (!_outboxDic.TryGetValue(itemMeta.OutboxId, out var outbox_list))
                {
                    outbox_list = [];
                    _outboxDic.TryAdd(itemMeta.OutboxId, outbox_list);
                }
                outbox_list.Add(itemMeta);
            }
            else
            {
                // 未指定发件箱
                _anoymousList.Add(itemMeta);
            }

            // 添加计数
            if (!fromRecycle)
            {
                Counter.IncreaseTotalCount();
            }

            return true;
        }

        /// <summary>
        /// 移到回收站
        /// </summary>
        /// <param name="itemMeta"></param>
        /// <returns></returns>
        public bool MoveToRecycleBin(SendItemMeta itemMeta)
        {
            return _recycleBin.TryAdd(itemMeta.SendingItemId, itemMeta);
        }

        /// <summary>
        /// 从回收站中移动到待发件项
        /// </summary>
        /// <param name="sendingItemId"></param>
        /// <returns></returns>
        public bool MoveRecycleToWaitList(long sendingItemId)
        {
            if (!_recycleBin.TryGetValue(sendingItemId, out var meta)) return false;
            _recycleBin.TryRemove(sendingItemId, out _);
            return Add(meta, true);
        }

        /// <summary>
        /// 移除回收站中的项
        /// </summary>
        /// <param name="sendingItemId"></param>
        /// <returns></returns>
        public bool ClearRecycleBin(long sendingItemId, bool success)
        {
            if (!_recycleBin.TryRemove(sendingItemId, out _)) return false;
            Counter.IncreaseSentCount(success);
            return true;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool AddRange(IEnumerable<SendItemMeta> items)
        {
            if (items == null) return false;
            foreach (var meta in items)
            {
                Add(meta);
            }
            return true;
        }

        /// <summary>
        /// 添加待发件项
        /// </summary>
        /// <param name="sendingItemId"></param>
        /// <returns></returns>
        public bool Add(long sendingItemId)
        {
            return Add(new SendItemMeta(sendingItemId));
        }

        /// <summary>
        /// 添加发件项并指定发件箱
        /// </summary>
        /// <param name="sendingItemId"></param>
        /// <param name="outboxId"></param>
        /// <returns></returns>
        public bool Add(long sendingItemId, long outboxId)
        {
            var meta = new SendItemMeta(sendingItemId, outboxId);
            return Add(meta);
        }

        /// <summary>
        /// 获取发件箱
        /// </summary>
        /// <returns></returns>
        public SendItemMeta? GetSendingMeta()
        {
            if (_anoymousList.TryTake(out var meta))
            {
                _allWaitDic.TryRemove(meta.SendingItemId, out _);
                MoveToRecycleBin(meta);

                return meta;
            }

            return null;
        }

        /// <summary>
        /// 通过 outboxId 来获取指定的发件元数据
        /// </summary>
        /// <param name="outboxId"></param>
        /// <returns></returns>
        public SendItemMeta? GetSendingMeta(long outboxId)
        {
            if (!_outboxDic.TryGetValue(outboxId, out var list)) return null;

            if (!list.TryTake(out var meta)) return null;

            _allWaitDic.TryRemove(meta.SendingItemId, out _);
            MoveToRecycleBin(meta);

            return meta;
        }

        /// <summary>
        /// 向数据库请求发送项
        /// 此处会批量请求，从而进行缓存
        /// </summary>
        /// <param name="sendItemMeta"></param>
        /// <returns></returns>
        public async Task<SendItemMeta> FillSendingItem(SqlContext sqlContext, SendItemMeta sendItemMeta)
        {
            if (sendItemMeta.SendingItem != null) return sendItemMeta;

            // 批量从数据库中提取数据，每次取 100 条
            var sendingItemMetas = _allWaitDic.Values.Where(x => x.SendingItem == null).Take(100).ToList();
            sendingItemMetas.Add(sendItemMeta);

            var sendingItemIds = sendingItemMetas.Select(x => x.SendingItemId).ToList();
            var sendingItems = await sqlContext.SendingItems
                .AsNoTracking()
                .Where(x => sendingItemIds.Contains(x.Id))
                .Include(x => x.Attachments)
                .ToListAsync();

            // 更新到元数据中
            foreach (var meta in sendingItemMetas)
            {
                var sendingItem = sendingItems.FirstOrDefault(x => x.Id == meta.SendingItemId);
                if (sendingItem == null) continue;
                meta.SetSendingItem(sendingItem);
            }

            return sendItemMeta;
        }
    }
}
