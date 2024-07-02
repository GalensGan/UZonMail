using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.MultiTenant;
using UZonMailService.Services.EmailSending.Base;
using UZonMailService.Services.EmailSending.Event;
using UZonMailService.Services.EmailSending.Event.Commands;
using UZonMailService.Services.EmailSending.Pipeline;

namespace UZonMailService.Services.EmailSending.OutboxPool
{
    /// <summary>
    /// 所有用户的发件箱池管理器
    /// </summary>
    public class UserOutboxesPoolManager : ISingletonService
    {
        private IServiceScopeFactory _ssf;
        private DictionaryManager<UserOutboxesPool> _userOutboxesPools = new();
        public UserOutboxesPoolManager(IServiceScopeFactory ssf)
        {
            this._ssf = ssf;
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
                    Message = "发件池为空"
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
            var result = await data.Data.GetOutboxByWeight(sendingContext);
            sendingContext.OutboxEmailAddress = result.Data;
            return result;
        }

        /// <summary>
        /// 添加发件箱
        /// </summary>
        /// <param name="outbox"></param>
        public async Task AddOutbox(OutboxEmailAddress outbox)
        {
            // 不存在，添加            
            if (!_userOutboxesPools.TryGetValue(outbox.Key, out var value))
            {
                var outboxPool = new UserOutboxesPool(_ssf, outbox.UserId);
                await outboxPool.AddOutbox(outbox);
                _userOutboxesPools.TryAdd(outbox.Key, outboxPool);
                return;
            }

            // 调用下级进行添加
            await value.AddOutbox(outbox);
        }

        public async Task EmailItemSendCompleted(SendingContext sendingContext)
        {
            // 移除发件箱
            if (sendingContext.UserOutboxesPool.IsEmpty)
            {
                _userOutboxesPools.TryRemove(sendingContext.UserOutboxesPool.UserId.ToString(), out _);
            }
        }

        public async Task RemoveOutboxesBySendingGroup(long userId, long sendingGroupId)
        {
            if (!_userOutboxesPools.TryGetValue(userId.ToString(), out var userOutboxesPool)) return;

        }
    }
}
