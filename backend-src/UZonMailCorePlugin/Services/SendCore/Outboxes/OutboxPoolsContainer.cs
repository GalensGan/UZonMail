using log4net;
using System.Collections.Concurrent;
using System.Runtime.Intrinsics.X86;
using UZonMail.Core.Services.EmailSending.Base;
using UZonMail.Core.Services.EmailSending.OutboxPool;
using UZonMail.Core.Services.SendCore.Contexts;
using UZonMail.Core.Services.SendCore.Utils;
using UZonMail.Core.Services.UzonMailCore.Utils;
using UZonMail.Utils.Web.Service;

namespace UZonMail.Core.Services.SendCore.Outboxes
{
    /// <summary>
    /// 发件箱池
    /// </summary>
    public class OutboxPoolsContainer(ServiceProvider serviceProvider) : ISingletonService
    {
        private readonly static ILog _logger = LogManager.GetLogger(typeof(UserOutboxesPoolManager));

        // key : 用户id，value: 发件箱池
        private readonly ConcurrentDictionary<long, OutboxesPool> _pools = new();

        // 权重计算器
        private readonly WeightCalculator<long> _weightCalculator = new();

        /// <summary>
        /// 添加发件箱
        /// </summary>
        /// <param name="outbox"></param>
        public async Task AddOutbox(OutboxEmailAddress outbox,)
        {
            var outboxPool = new OutboxesPool(outbox.UserId, outbox.Weight);
            if (!_pools.TryAdd(outbox.UserId, outboxPool))
            {
                // 若存在，获取既有的发件箱池
                var existOutboxPool = _pools[outbox.UserId];
                
                // 更新权重
            }

            // 向发件箱池中添加发件箱
            outboxPool.AddOutbox(outbox);
        }

        /// <summary>
        /// 通过用户发件池的权重先筛选出发件池，然后从这个用户的发件池中选择一个发件箱
        /// </summary>
        /// <returns></returns>
        public OutboxEmailAddress? GetOutbox()
        {
            if (this.Count == 0)
            {
                _logger.Info("系统发件池为空");
                return null;
            }

            var data = this.GetDataByWeight();

            // 未获取到发件箱
            if (!data)
            {
                return null;
            }

            // 保存当前引用
            sendingContext.UserOutboxesPoolManager = this;

            // 获取子项
            var userOutboxesPool = data.Data as OutboxesPool;
            var result = await userOutboxesPool.GetOutboxByWeight(sendingContext);
            sendingContext.OutboxEmailAddress = result.Data;
            return result;
        }



        /// <summary>
        /// 邮件发送完成回调
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        public async Task EmailItemSendCompleted(SendingContext sendingContext)
        {
            // 移除发件箱
            if (sendingContext.UserOutboxesPool.IsEmpty)
            {
                UserOutboxesPools.TryRemove(sendingContext.UserOutboxesPool.UserId, out _);
                _logger.Info($"用户 {sendingContext.UserOutboxesPool.UserId} 发件池为空，从发件池管理器中移除");
            }
        }

        /// <summary>
        /// 通过用户 Id 和邮件组id 移除关联的发件箱
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sendingGroupId"></param>
        /// <returns></returns>
        public async Task RemoveOutboxesBySendingGroup(long userId, long sendingGroupId)
        {
            if (!UserOutboxesPools.TryGetValue(userId, out var userOutboxesPool)) return;

            // 找到后，开始执行操作
            var outboxes = userOutboxesPool.Values.Where(x => x.SendingGroupIds.Contains(sendingGroupId));
            // 开始移除
            foreach (var outbox in outboxes)
            {
                outbox.SendingGroupIds.Remove(sendingGroupId);
                if (outbox.SendingGroupIds.Count != 0) continue;

                // 标记为使用中，防止其它线程继续使用
                outbox.LockUsing();
                // 发件箱没有对应的发件组，移除
                userOutboxesPool.TryRemove(outbox.Email, out _);
            }
        }
    }
}
