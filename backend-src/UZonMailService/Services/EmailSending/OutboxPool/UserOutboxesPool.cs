using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SQL.Emails;

namespace UZonMailService.Services.EmailSending.OutboxPool
{
    /// <summary>
    /// 用户的发件箱池
    /// 每个邮箱账号共用冷却池
    /// key: 用户 id，value: 发件箱列表
    /// </summary>
    public class UserOutboxesPool : ConcurrentDictionary<int, List<OutboxEmailAddress>>, ISingletonService
    {
        /// <summary>
        /// 发件箱冷却时间结束
        /// </summary>
        public event Action<int> OutboxCoolDownFinish;

        /// <summary>
        /// 发件箱达到发件限制，没释放事件
        /// </summary>
        public event Action<OutboxEmailAddress> OutboxDisposed;

        /// <summary>
        /// 添加发件箱组
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="outbox"></param>
        public void AddOutbox(int userId, OutboxEmailAddress outbox)
        {
            if (!this.TryGetValue(userId, out var value))
            {
                value = [outbox];
                this[userId] = value;
                return;
            }

            // 存在则替换，在此时更新发件箱设置
            int existIndex = value.FindIndex(x => x.Id == outbox.Id);
            if (existIndex >= 0)
            {
                value[existIndex].Update(outbox);
            }
            else value.Add(outbox);
        }

        /// <summary>
        /// 移除发件箱
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        public void RemoveOutbox(int userId, int groupId)
        {
            if (!this.TryGetValue(userId, out var value)) return;

            // 建立快照，防止程序又向里面添加
            var tempBoxes = value.ToArray();
            foreach(var box in tempBoxes)
            {
                box.SendingGroupIds.Remove(groupId);
                if (box.SendingGroupIds.Count == 0)
                {
                    // 从原始数据中移除
                    value.Remove(box);
                    // 释放资源
                    box.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取发件箱数量
        /// </summary>
        /// <returns></returns>
        public int GetOutboxesCount()
        {
            var temps = this.ToArray();
            return temps.Sum(o => o.Value.Count);
        }

        /// <summary>
        /// 获取发件组可用的发件箱数量
        /// </summary>
        /// <param name="sendingGroupId"></param>
        /// <returns></returns>
        public int GetOutboxesCount(int sendingGroupId)
        {
           var temps = this.ToArray();
            return temps.Sum(o => o.Value.Count(x => !x.ShouldDispose && x.SendingGroupIds.Contains(sendingGroupId)));
        }

        /// <summary>
        /// 获取发件箱组
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="sendingGroupId">发件组id</param>
        /// <param name="outboxId">发件箱id,若有，则只获取该发件箱来发件</param>
        /// <param name="status">0 未匹配到，1 正常, 2 冷却中</param>
        /// <returns></returns>
        public OutboxEmailAddress? GetOutbox(int userId, int sendingGroupId, int outboxId, out int status)
        {
            status = 2;
            var outboxes = this[userId].ToArray();
            OutboxEmailAddress? result;
            if (outboxId > 0)
            {
                result = outboxes.FirstOrDefault(o => o.Enable && o.Id == outboxId);
                status = 0;
                return null;
            }
            else
            {
                result = outboxes.FirstOrDefault(o => o.Enable && o.SendingGroupIds.Contains(sendingGroupId));
            }
           
            if(result==null) return null;

            // 取出来后，进入冷却时间
            // 避免被其它线程取出
            result.SetCooldown(OutboxCoolDownFinishHandler);
            if (result.ShouldDispose)
            {
                // 移除发件箱
                this[userId].Remove(result);
                // 触发事件
                OutboxDisposed?.Invoke(result);
                result.Dispose();
            }
            status = 1;
            return result;
        }

        /// <summary>
        /// 触发冷却结束事件
        /// </summary>
        /// <param name="count"></param>
        private void OutboxCoolDownFinishHandler(int count)
        {
            OutboxCoolDownFinish?.Invoke(count);
        }

        /// <summary>
        /// 获取用户内存中的发件箱列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<OutboxEmailAddress> GetExistOutboxes(int userId)
        {
            if (!this.TryGetValue(userId, out var value)) return [];
            return value;
        }
    }
}
