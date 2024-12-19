﻿using Microsoft.EntityFrameworkCore;
using UZonMail.Core.Services.EmailSending.Sender;
using UZonMail.Core.SignalRHubs.SendEmail;
using UZonMail.Core.SignalRHubs.Extensions;
using UZonMail.Core.Utils.Database;
using log4net;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.DB.SQL.Emails;
using UZonMail.DB.SQL.Templates;
using UZonMail.Core.Database.SQL.EmailSending;
using UZonMail.DB.SQL;

using UZonMail.Core.Services.SendCore.Outboxes;
using UZonMail.Core.Services.SendCore.Contexts;
using UZonMail.Core.Services.SendCore.EmailWaitList;
using UZonMail.Core.Services.SendCore.WaitList;
using UZonMail.DB.Managers.Cache;
using UZonMail.DB.MySql;
using UZonMail.DB.SQL.Base;

namespace UZonMail.Core.Services.EmailSending.WaitList
{
    /// <summary>
    /// 一次发件任务
    /// 不同的发件任务包含不同的数据库上下文，保证上下文不重复使用
    /// 包含发件组和收件内容
    /// </summary>
    /// <remarks>
    /// 构造
    /// </remarks>
    public class GroupTask
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(GroupTask));
        private readonly object _lockObject = new();

        private GroupTask(long sendingGroupId, List<string> smtpPasswordSecretKeys)
        {
            SendingGroupId = sendingGroupId;
            SmtpPasswordSecretKeys = smtpPasswordSecretKeys;
        }

        /// <summary>
        /// 创建一个发件任务
        /// </summary>
        /// <param name="scopeServices"></param>
        /// <param name="sendingGroupId"></param>
        /// <param name="smtpPasswordSecretKeys"></param>
        /// <returns></returns>
        public static async Task<GroupTask?> Create(SendingContext scopeServices
        , long sendingGroupId
        , List<string> smtpPasswordSecretKeys
        )
        {
            GroupTask groupTask = new(sendingGroupId, smtpPasswordSecretKeys);
            // 初始化组
            if (!await groupTask.InitSendingGroup(scopeServices)) return null;
            return groupTask;
        }

        #region 属性
        /// <summary>
        /// 组 id
        /// </summary>
        public long SendingGroupId { get; private set; }

        /// <summary>
        /// 通过组 id 获取的组
        /// </summary>
        private SendingGroup _sendingGroup;

        /// <summary>
        /// 密钥
        /// </summary>
        public List<string> SmtpPasswordSecretKeys { get; private set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        public long UserId { get; private set; }

        /// <summary>
        /// 发件项数据
        /// 此处只保存自由发件的项
        /// 若指定发件箱，数据 id 会保存在 outbox 中
        /// </summary>
        private readonly SendingItemMetaList _sendingItemMetas = new();

        /// <summary>
        /// 可用的代理
        /// </summary>
        private UsableProxyList _usableProxies;

        /// <summary>
        /// 可用的模板
        /// </summary>
        private UsableTemplateList _usableTemplates;

        /// <summary>
        /// 是否应该释放
        /// </summary>
        public bool ShouldDispose => _sendingItemMetas.ToSendingCount == 0;

        /// <summary>
        /// 已成功的数量
        /// </summary>
        private DateTime _startDate = DateTime.Now;
        #endregion


        /// <summary>
        /// 初始化发件组信息
        /// 只能被调用一次
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        private async Task<bool> InitSendingGroup(SendingContext sendingContext)
        {
            var sqlContext = sendingContext.Provider.GetRequiredService<SqlContext>();

            // 获取完整的邮件组
            var sendingGroup = await sqlContext.SendingGroups
                .AsNoTracking()
                .Where(x => x.Id == SendingGroupId)
                .Include(x => x.Outboxes)
                .Include(x => x.Templates)
                .FirstOrDefaultAsync();

            if (sendingGroup == null)
            {
                _logger.Error($"发件组 {SendingGroupId} 不存在");
                return false;
            }
            // 保存发件组
            _sendingGroup = sendingGroup;

            // 将收件箱重置为空，方便垃圾回收
            sendingGroup.Inboxes = [];

            // 正在发送时，不添加
            if (sendingGroup.Status == SendingGroupStatus.Sending)
            {
                _logger.Error($"发件组 {SendingGroupId} 正在发送中");
                return false;
            }

            // 更新用户 id
            UserId = sendingGroup.UserId;

            // 将公共的发件箱添加到发件池中
            // 邮件级别的发件箱在初始化发送项时，再添加
            await AddSharedOutboxToPool(sendingContext, sendingGroup.Outboxes, sendingGroup.OutboxGroups);

            // 获取全部代理，代理是组织级别的
            this._usableProxies = new UsableProxyList(UserId);
            // 获取所有的模板，模板是用户级别的
            this._usableTemplates = new UsableTemplateList(UserId);
            return true;
        }

        /// <summary>
        /// 将新的发件箱添加到发件池中
        /// 会自动去重
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <param name="outboxes"></param>
        /// <param name="outboxGroup"></param>
        /// <returns></returns>
        private async Task AddSharedOutboxToPool(SendingContext sendingContext, List<Outbox> outboxes, List<IdAndName>? outboxGroup)
        {
            if (outboxes.Count == 0) return;
            var container = sendingContext.Provider.GetRequiredService<OutboxesPoolList>();

            var outboxAddresses = outboxes.ConvertAll(x => new OutboxEmailAddress(x, SendingGroupId, SmtpPasswordSecretKeys, OutboxEmailAddressType.Shared));
            foreach (var outbox in outboxAddresses)
            {
                await container.AddOutbox(outbox);
            }

            // 解析发件箱组
            if (outboxGroup == null || outboxGroup.Count == 0) return;
            var outboxGroupIds = outboxGroup.Select(x => x.Id).ToList();
            // 添加发件组的发件箱
            var sqlContext = sendingContext.Provider.GetRequiredService<SqlContext>();
            var groupBoxes = await sqlContext.Outboxes.AsNoTracking()
                .Where(x => outboxGroupIds.Contains(x.EmailGroupId)).ToListAsync();
            await AddSharedOutboxToPool(sendingContext, groupBoxes, null);
        }


        /// <summary>
        /// 初始化发件组的发件项
        /// 允许被被多次调用
        /// 多次调用的场景: 重发部分发件项
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <param name="sendingItemIds">只发送特定的发件项</param>
        /// <returns></returns>
        public async Task<bool> InitSendingItems(SendingContext sendingContext, List<long>? sendingItemIds)
        {
            var sqlContext = sendingContext.Provider.GetRequiredService<SqlContext>();
            // 获取待发件
            var dbSet = sqlContext.SendingItems.AsNoTracking()
                .Where(x => x.SendingGroupId == SendingGroupId)
                .Where(x => (x.Status & SendingItemStatus.CanSend) == SendingItemStatus.CanSend); // 获取可发送项

            if (sendingItemIds != null && sendingItemIds.Count > 0)
            {
                dbSet = dbSet.Where(x => sendingItemIds.Contains(x.Id));
            }
            List<SendingItem> toSendingItems = await dbSet.Select(x => new SendingItem() { Id = x.Id, OutBoxId = x.OutBoxId, Inboxes = x.Inboxes }).ToListAsync();

            // 去掉当前组中已经包含的项
            var existIds = _sendingItemMetas.SendingItemIds.ToList();
            toSendingItems = toSendingItems.Where(x => !existIds.Contains(x.Id)).ToList();

            // 获取发件箱
            HashSet<long> outboxIds = toSendingItems.Select(x => x.OutBoxId).Where(x => x > 0).ToHashSet();
            var outboxes = await sqlContext.Outboxes.Where(x => outboxIds.Contains(x.Id)).ToListAsync();

            // 对于找不到发件项的邮件，给予标记非法
            var invalidSendingItemIds = toSendingItems.Where(x => x.OutBoxId > 0)
                .Where(x =>
                {
                    var outbox = outboxes.FirstOrDefault(o => o.Id == x.OutBoxId);
                    return outbox == null;
                })
                .Select(x => x.Id)
                .ToList();
            if (invalidSendingItemIds.Count > 0)
            {
                // 更新邮件状态
                await sqlContext.SendingItems.UpdateAsync(x => invalidSendingItemIds.Contains(x.Id),
                    x => x.SetProperty(y => y.Status, SendingItemStatus.Invalid | SendingItemStatus.Failed)
                        .SetProperty(y => y.SendDate, DateTime.Now)
                        .SetProperty(y => y.SendResult, "指定的发件箱已被删除"));

                // 过滤掉无效的发件箱项
                toSendingItems = [.. toSendingItems.FindAll(x => !invalidSendingItemIds.Contains(x.Id))];
            }

            // 对于取消订阅的邮件，进行标记
            var userInfo = await DBCacher.GetCache<UserInfoCache>(sqlContext, UserId);
            var unsubscribedEmails = await sqlContext.UnsubscribeEmails.AsNoTracking()
                .Where(x => x.OrganizationId == userInfo.OrganizationId)
                .Where(x => !x.IsDeleted)
                .Select(x => x.Email)
                .ToListAsync();
            if (unsubscribedEmails.Count > 0)
            {
                var unsubscribedSendingItemIds = toSendingItems.Where(x => x.Inboxes.Any(i => unsubscribedEmails.Contains(i.Email))).Select(x => x.Id).ToList();
                if (unsubscribedSendingItemIds.Count > 0)
                {
                    // 更新邮件状态
                    await sqlContext.SendingItems.UpdateAsync(x => unsubscribedSendingItemIds.Contains(x.Id),
                                               x => x.SetProperty(y => y.Status, SendingItemStatus.Unsubscribed | SendingItemStatus.Failed)
                                                     .SetProperty(y => y.SendDate, DateTime.Now)
                                                     .SetProperty(y => y.SendResult, "收件人已取消订阅"));
                    toSendingItems = toSendingItems.FindAll(x => !unsubscribedSendingItemIds.Contains(x.Id));
                }
            }

            // 更新待发件列表
            // 由于初始化时，不是并发的，不需要加锁
            var toSendingItemMetas = toSendingItems.Select(x => new SendItemMeta(x.Id, x.OutBoxId)).ToList();
            _sendingItemMetas.AddRange(toSendingItemMetas);

            // 更新当前发件组的数据 
            await UpdateSendingGroupInfo(sqlContext, SendingGroupId);

            // 新增特定发件箱
            var outboxesPool = sendingContext.Provider.GetRequiredService<OutboxesPool>();
            foreach (var outbox in outboxes)
            {
                // 获取收件项 Id
                var sendingItemIdsTemp = toSendingItems.Where(x => x.OutBoxId == outbox.Id).Select(x => x.Id).ToList();
                var outboxAddress = new OutboxEmailAddress(outbox, SendingGroupId, SmtpPasswordSecretKeys, OutboxEmailAddressType.Specific, sendingItemIdsTemp);
                outboxesPool.AddOutbox(outboxAddress);
            }

            // 将发件项修改为发送中
            if (toSendingItems.Count > 0)
            {
                await sqlContext.SendingItems.UpdateAsync(x => toSendingItems.Select(x => x.Id).Contains(x.Id),
                    x => x.SetProperty(y => y.Status, SendingItemStatus.Pending));
            }

            // 通知用户，任务已开始
            var client = sendingContext.HubClient.GetUserClient(UserId);
            await client.SendingGroupProgressChanged(new SendingGroupProgressArg(_sendingGroup, _startDate)
            {
                ProgressType = ProgressType.Start
            });

            return true;
        }

        /// <summary>
        /// 更新发件组信息，并保存到数据库中
        /// </summary>
        /// <param name="sqlContext"></param>
        /// <param name="sendingGroupId"></param>
        /// <returns></returns>
        private async Task UpdateSendingGroupInfo(SqlContext sqlContext, long sendingGroupId)
        {
            // 获取成功数、失败数、总数
            var sendingGroup = await sqlContext.SendingGroups.Where(x => x.Id == sendingGroupId).FirstOrDefaultAsync();
            if (sendingGroup == null) return;

            var allSendingItems = await sqlContext.SendingItems.AsNoTracking()
                .Where(x => x.SendingGroupId == sendingGroup.Id)
                .Select(x => new
                {
                    x.Status,
                })
                .ToListAsync();

            // 更新发件组的信息
            sendingGroup.Status = SendingGroupStatus.Sending;
            sendingGroup.TotalCount = allSendingItems.Count;
            // 不能发送的项皆当作成功项
            sendingGroup.SuccessCount = allSendingItems.Count(x => x.Status.HasFlag(SendingItemStatus.Success));
            sendingGroup.SentCount = allSendingItems.Count(x => !x.Status.HasFlag(SendingItemStatus.CanSend));
            // 开始发送日期
            if (sendingGroup.SendStartDate == DateTime.MinValue)
                sendingGroup.SendStartDate = DateTime.Now;
            // 保存组状态
            await sqlContext.SaveChangesAsync();


            // 更新到当前类中
            _sendingGroup.Status = sendingGroup.Status;
            _sendingGroup.TotalCount = sendingGroup.TotalCount;
            _sendingGroup.SuccessCount = sendingGroup.SuccessCount;
            _sendingGroup.SentCount = sendingGroup.SentCount;
            _sendingGroup.SendStartDate = sendingGroup.SendStartDate;
        }

        /// <summary>
        /// 获取发件项
        /// </summary>
        /// <returns></returns>
        public async Task<SendItemMeta?> GetEmailItem(SendingContext sendingContext)
        {
            var outboxEmailAddress = sendingContext.OutboxAddress;
            if (outboxEmailAddress == null) return null;

            // 判断是否为当前组对应的发件箱
            if (!outboxEmailAddress.ContainsSendingGroup(SendingGroupId)) return null;

            // 从列表中移除发件项并转换成 sendItem
            var sendItemMeta = await GetEmailItemFromDb(sendingContext);
            if (sendItemMeta == null) return null;

            // 为 sendItem 动态赋值
            // 赋予发件箱
            sendItemMeta.SetOutbox(outboxEmailAddress);

            var sqlContext = sendingContext.SqlContext;
            await sendItemMeta.SetReplyToEmails(sqlContext, outboxEmailAddress.ReplyToEmails);

            // 推送开始发件
            var client = sendingContext.HubClient.GetUserClient(UserId);
            client?.SendingItemStatusChanged(new SendingItemStatusChangedArg(sendItemMeta.SendingItem)
            {
                Status = SendingItemStatus.Sending
            });

            return sendItemMeta;
        }

        /// <summary>
        /// 从数据库中获取发件项
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        private async Task<SendItemMeta?> GetEmailItemFromDb(SendingContext sendingContext)
        {
            var outbox = sendingContext.OutboxAddress ?? throw new Exception("GetSendItem 调用失败, 请先获取发件箱");

            // 先发指定项
            SendItemMeta? sendItemMeta = null;
            if (outbox.Type.HasFlag(OutboxEmailAddressType.Specific))
            {
                // 获取特定项
                sendItemMeta = _sendingItemMetas.GetSendingMeta(outbox.Id);
            }

            // 从当前组中获取
            if (outbox.Type.HasFlag(OutboxEmailAddressType.Shared) && sendItemMeta == null)
            {
                sendItemMeta = _sendingItemMetas.GetSendingMeta();
            }

            if (sendItemMeta == null) return null;
            // 如果已经包含 SendingItem, 说明初始化过了，直接返回
            if (sendItemMeta.SendingItem != null)
            {
                return sendItemMeta;
            }

            // 拉取发件项
            var sqlContext = sendingContext.SqlContext;
            sendItemMeta = await _sendingItemMetas.FillSendingItem(sqlContext, sendItemMeta);

            // 获取附件
            await sendItemMeta.ResolveAttachments(sendingContext);

            // 生成正文原始内容
            var originBody = await GetSendingItemOriginBody(sqlContext, sendItemMeta.SendingItem, sendItemMeta.BodyData);
            await sendItemMeta.SetHtmlBody(sendingContext, originBody);

            // 设置主题
            var subject = GetSubject(sendItemMeta.BodyData);
            sendItemMeta.SetSubject(subject);

            // 添加最大重试次数
            await sendItemMeta.SetMaxRetryCount(sqlContext);

            // 添加代理
            // 代理与发件箱进行匹配            
            var proxyInfo = await _usableProxies.GetProxy(sqlContext, sendItemMeta.SendingItemId, outbox.Email);
            sendItemMeta.SetProxyInfo(proxyInfo);

            return sendItemMeta;
        }


        /// <summary>
        /// 获取原始发件内容
        /// 变量未经过处理
        /// </summary>
        /// <param name="sendItemMeta"></param>
        /// <param name="templates"></param>
        /// <returns></returns>
        private async Task<string> GetSendingItemOriginBody(SqlContext sqlContext, SendingItem sendingItem, SendingItemExcelData? bodyData)
        {
            // 批量发送的情况
            if (sendingItem.IsSendingBatch)
            {
                if (!string.IsNullOrEmpty(_sendingGroup.Body)) return _sendingGroup.Body;

                var template = await _usableTemplates.GetTemplate(sqlContext, sendingItem.Id);
                return template?.Content ?? string.Empty;
            }

            // 非批量发送
            // 当有用户数据时
            if (bodyData != null)
            {
                // 判断是否有 body
                if (!string.IsNullOrEmpty(bodyData.Body)) return bodyData.Body;

                // 判断是否有模板 id 或者模板名称
                if (bodyData.TemplateId > 0)
                {
                    var template = await _usableTemplates.GetTemplateById(sqlContext, bodyData.TemplateId);
                    if (template != null) return template.Content;
                }

                // 判断是否有模板名称
                if (!string.IsNullOrEmpty(bodyData.TemplateName))
                {
                    var template = await _usableTemplates.GetTemplateByName(sqlContext, bodyData.TemplateName);
                    if (template != null) return template.Content;
                }
            }

            // 没有数据时，优先使用组中的 body
            if (!string.IsNullOrEmpty(_sendingGroup.Body)) return _sendingGroup.Body;

            // 返回随机模板
            var randTemplate = await _usableTemplates.GetTemplate(sqlContext, sendingItem.Id);
            return randTemplate?.Content ?? string.Empty;
        }

        /// <summary>
        /// 获取主题
        /// 随机主题只需要在发送时确定即可
        /// </summary>
        /// <returns></returns>
        private string GetSubject(SendingItemExcelData? bodyData)
        {
            // 若本身有主题，则使用自身的主题
            if (bodyData != null && !string.IsNullOrEmpty(bodyData.Subject))
            {
                return bodyData.Subject;
            }
            return _sendingGroup.GetRandSubject();
        }

        /// <summary>
        /// 判断是否在冷却中
        /// </summary>
        /// <param name="inboxIds"></param>
        /// <returns></returns>
        private async Task<Tuple<bool, string>> IsInboxICooling(SqlContext sqlContext, List<long> inboxIds)
        {
            // 获取收件箱信息
            var inboxes = await sqlContext.Inboxes.Where(x => inboxIds.Contains(x.Id))
                .ToListAsync();
            if (inboxes.Count == 0) return Tuple.Create(false, "");

            // 自身有冷却设置
            var inboxesWithSelfCooling = inboxes.Where(x => x.MinInboxCooldownHours >= 0).ToList();
            var coolingInbox = inboxesWithSelfCooling.FirstOrDefault(x => x.LastSuccessDeliveryDate.AddHours(x.MinInboxCooldownHours) > DateTime.Now);
            if (coolingInbox != null)
            {
                return Tuple.Create(true, $"收件箱 {coolingInbox.Email} 正在冷却中");
            }

            // 从缓存中读取设置数据
            var userReader = await DBCacher.GetCache<UserInfoCache>(sqlContext, UserId.ToString());
            var settingsReader = await DBCacher.GetCache<OrganizationSettingCache>(sqlContext, userReader.OrganizationObjectId);

            if (settingsReader.MinInboxCooldownHours <= 0) return Tuple.Create(false, "");

            var inboxesWhithGlobalCooling = inboxes.Where(x => x.MinInboxCooldownHours < 0).ToList();
            coolingInbox = inboxesWhithGlobalCooling.FirstOrDefault(x => x.LastSuccessDeliveryDate.AddHours(settingsReader.MinInboxCooldownHours) > DateTime.Now);
            if (coolingInbox != null)
            {
                return Tuple.Create(true, $"收件箱 {coolingInbox.Email} 正在冷却中");
            }

            // 从数据库中判断
            return Tuple.Create(false, "");
        }



        /// <summary>
        /// 邮件发送完成
        /// 只储存 邮件组 数据，具体的每次发件数据在 SendItem 中处理
        /// 只有失败或者成功才会触发，重发时不会触发
        /// </summary>
        /// <param name="sendingContext">发送结果</param>
        public async Task EmailItemSendCompleted(SendingContext sendingContext)
        {
            // 若是 outbox 连接错误，则移除所有相关发件项
            if (sendingContext.SendResult.SentStatus.HasFlag(SentStatus.OutboxError))
            {
                await RemoveSpecificSendingItems(sendingContext);
            }

            // 移除运行计数
            _sendingItemsCounter.IncreaseRunningCount(-1);

            var sendCompleteResult = sendingContext.SendResult;
            var sendItem = sendingContext.EmailItem;

            // 若需要重试，则重新添加到队列中
            if (sendCompleteResult.SentStatus.HasFlag(SentStatus.Retry))
            {
                _sendingItemMetas.Add(sendingContext.EmailItem.SendItemMeta);
            }
            else
            {
                // 只有已经发送成功或者不重试了，才更新发送进度
                _sendingItemsCounter.IncreaseSentCount(sendCompleteResult.Ok);
                var sqlContext = sendingContext.SqlContext;

                // 向数据库中保存状态
                var newSendingGroup = await sqlContext.SendingGroups.FirstAsync(x => x.Id == _sendingGroup.Id);
                newSendingGroup.SuccessCount = _sendingItemsCounter.TotalSuccessCount;
                newSendingGroup.SentCount = _sendingItemsCounter.TotalSentCount;
                newSendingGroup.LastMessage = sendCompleteResult.Message;
                await sqlContext.SaveChangesAsync();

                // 向用户推送发送组的进度            
                var client = sendingContext.HubClient.GetUserClient(UserId);
                if (client != null)
                {
                    // 推送发送组进度
                    await client.SendingGroupProgressChanged(new SendingGroupProgressArg(newSendingGroup, _startDate));

                    // 若已经完成，推送完成的进度
                    if (ShouldDispose)
                    {
                        await client.SendingGroupProgressChanged(new SendingGroupProgressArg(newSendingGroup, _startDate)
                        {
                            ProgressType = ProgressType.End
                        });
                    }
                }
            }

            // 调用外部回调
            await sendingContext.UserSendingGroupsPool.EmailItemSendCompleted(sendingContext);
        }

        /// <summary>
        /// 移除指定发件箱的发件项
        /// </summary>
        /// <param name="outboxEmail"></param>
        /// <returns></returns>
        public async Task RemoveSpecificSendingItems(SendingContext sendingContext)
        {
            var outboxEmail = sendingContext.OutboxEmailAddress;
            if (outboxEmail.SendingItemIds.IsEmpty) return;

            _sendingItemsCounter.IncreaseTotalCount(-outboxEmail.SendingItemIds.Count);
            // 对这些发件项标记失败
            await sendingContext.SqlContext.SendingItems.UpdateAsync(x => outboxEmail.SendingItemIds.Contains(x.Id),
                x => x.SetProperty(y => y.Status, SendingItemStatus.Failed)
                .SetProperty(y => y.SendDate, DateTime.Now)
                .SetProperty(y => y.SendResult, sendingContext.SendResult.Message));
        }

        public async Task NotifyEnd(SendingContext sendingContext, long sendingGroupId)
        {
            var client = sendingContext.HubClient.GetUserClient(UserId);
            if (client != null)
            {
                await client.SendingGroupProgressChanged(new SendingGroupProgressArg(sendingGroupId, _sendingItemsCounter, _startDate)
                {
                    ProgressType = ProgressType.End
                });
            }
        }
    }
}
