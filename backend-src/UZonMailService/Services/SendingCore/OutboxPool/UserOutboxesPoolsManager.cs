using log4net;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using UZonMail.Utils.UzonMail;
using UZonMail.Utils.Web.Service;
using UZonMailService.Controllers.SystemInfo.Model;
using UZonMailService.UzonMailDB.SQL;
using UZonMailService.UzonMailDB.SQL.Emails;
using UZonMailService.UzonMailDB.SQL.MultiTenant;
using UZonMailService.Services.EmailSending.Base;
using UZonMailService.Services.EmailSending.Event;
using UZonMailService.Services.EmailSending.Event.Commands;
using UZonMailService.Services.EmailSending.Pipeline;
using UZonMailService.Services.EmailSending.Utils;
using Uamazing.Utils.UzonMail;

namespace UZonMailService.Services.EmailSending.OutboxPool
{
    /// <summary>
    /// 所有用户的发件箱池管理器
    /// </summary>
    public class UserOutboxesPoolsManager : ISingletonService, ISendingStageBuilder, ISendingStage
    {
        private readonly static ILog _logger = LogManager.GetLogger(typeof(UserOutboxesPoolsManager));
        private IServiceScopeFactory _ssf;
        private readonly ConcurrentDictionary<long, UserOutboxesPool> _userOutboxesPools = new();

        public bool ShouldDispose => throw new NotImplementedException();

        public string StageName => throw new NotImplementedException();

        public UserOutboxesPoolsManager(IServiceScopeFactory ssf)
        {
            this._ssf = ssf;
        }

        /// <summary>
        /// 生成阶段
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        public async Task BuildSendingStage(ISendingContext sendingContext)
        {
            // 添加自己
            sendingContext.AddSendingStage(this);

            if (_userOutboxesPools.Count == 0)
            {
                var result = new SentResult(false, "系统发件池为空")
                {
                    SentStatus = SentStatus.EmptyOutboxesPoolsManager
                };
                sendingContext.SetSendResult(result);
                return;
            }

            var userOutboxesPoolResult = _userOutboxesPools.GetDataByWeight();
            // 未获取到发件箱
            if (!userOutboxesPoolResult)
            {
                var result = new SentResult(false, userOutboxesPoolResult.Message)
                {
                    SentStatus = SentStatus.Failed
                };
                sendingContext.SetSendResult(result);
                return;
            }

            // 获取子项
            if (userOutboxesPoolResult.Data is not UserOutboxesPool userOutboxesPool)
            {
                var result = new SentResult(false, "结果无法转成 UserOutboxesPool")
                {
                    SentStatus = SentStatus.Failed
                };
                sendingContext.SetSendResult(result);
                return;
            }
            sendingContext.AddSendingStage(userOutboxesPool);  
            await userOutboxesPool.BuildSendingStage(sendingContext);
        }

        /// <summary>
        /// 通过用户发件池的权重先筛选出发件池，然后从这个用户的发件池中选择一个发件箱
        /// </summary>
        /// <returns></returns>
        public async Task<FuncResult<OutboxEmailAddress>> GetOutboxByWeight(SendingContext sendingContext)
        {
            if (_userOutboxesPools.Count == 0)
                return new FuncResult<OutboxEmailAddress>()
                {
                    Ok = false,
                    Status = PoolResultStatus.EmptyError,
                    Message = "系统发件池为空"
                };

            var data = _userOutboxesPools.GetDataByWeight();

            // 未获取到发件箱
            if (!data)
                return new FuncResult<OutboxEmailAddress>()
                {
                    Ok = false,
                    Status = data.Status,
                    Message = data.Message
                };

            // 保存当前引用
            sendingContext.UserOutboxesPoolManager = this;

            // 获取子项
            var userOutboxesPool = data.Data as UserOutboxesPool;
            var result = await userOutboxesPool.GetOutboxByWeight(sendingContext);
            sendingContext.OutboxEmailAddress = result.Data;
            return result;
        }

        /// <summary>
        /// 添加发件箱
        /// </summary>
        /// <param name="outbox"></param>
        public async Task AddOutbox(SendingContext sendingContext, OutboxEmailAddress outbox)
        {
            // 不存在，添加            
            if (!_userOutboxesPools.TryGetValue(outbox.UserId, out var value))
            {
                // 获取用户信息
                var userInfo = await sendingContext.SqlContext.Users.Where(x => x.Id == outbox.UserId)
                    .Select(x => new { x.Weight })
                    .FirstOrDefaultAsync();

                var outboxPool = new UserOutboxesPool(_ssf, outbox.UserId, userInfo.Weight);
                await outboxPool.AddOutbox(outbox);
                _userOutboxesPools.TryAdd(outbox.UserId, outboxPool);
                return;
            }

            // 调用下级进行添加
            await value.AddOutbox(outbox);
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
                _userOutboxesPools.TryRemove(sendingContext.UserOutboxesPool.UserId, out _);
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
            if (!_userOutboxesPools.TryGetValue(userId, out var userOutboxesPool)) return;

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

        #region 统计分析
        public List<OutboxPoolInfo> GetOutboxPoolInfos()
        {
            return _userOutboxesPools.Values.Select(x =>
              {
                  return new OutboxPoolInfo(x);
              }).ToList();
        }

        public Task Execute(ISendingContext sendingContext)
        {
            throw new NotImplementedException();
        }

        public Task Dispose(ISendingContext sendingContext)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
