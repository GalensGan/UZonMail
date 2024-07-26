using log4net;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using UZonMail.Utils.Results;
using UZonMail.Utils.UzonMail;
using UZonMail.Utils.Web.Service;
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
    /// 单个用户的发件箱池
    /// 每个邮箱账号共用冷却池
    /// key: 邮箱 userId+邮箱号 ，value: 发件箱列表
    /// </summary>
    public class UserOutboxesPool : IWeight, ISendingStage, ISendingStageBuilder
    {
        private readonly static ILog _logger = LogManager.GetLogger(typeof(UserOutboxesPool));
        private readonly IServiceScopeFactory _ssf;
        private readonly ConcurrentDictionary<string, OutboxEmailAddress> _outboxes = new();

        public UserOutboxesPool(IServiceScopeFactory ssf, long userId, int weight)
        {
            _ssf = ssf;
            UserId = userId;
            Weight = weight > 0 ? weight : 1;
        }

        #region 自定义参数
        public long UserId { get; }
        #endregion

        #region 接口实现
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; private set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable
        {
            get
            {
                if (_outboxes.Count == 0) return false;
                return _outboxes.Values.Any(x => x.Enable);
            }
        }

        public bool ShouldDispose => throw new NotImplementedException();

        public string StageName => throw new NotImplementedException();
        #endregion

        /// <summary>
        /// 添加发件箱
        /// </summary>
        /// <param name="outbox"></param>
        public async Task<bool> AddOutbox(OutboxEmailAddress outbox)
        {
            if (_outboxes.TryGetValue(outbox.Email, out var existValue))
            {
                existValue.Update(outbox);
                return true;
            }

            // 验证发件箱是否有效
            if (!outbox.Validate())
            {
                return false;
            }


            // 不存在则添加
            _outboxes.TryAdd(outbox.Email, outbox);
            return true;
        }

        /// <summary>
        /// 按用户设置的权重获取发件箱
        /// </summary>
        /// <returns></returns>
        public async Task<FuncResult<OutboxEmailAddress>> GetOutboxByWeight(SendingContext scopeServices)
        {
            var data = _outboxes.GetDataByWeight();
            if (data.NotOk) return new FuncResult<OutboxEmailAddress>()
            {
                Message = data.Message,
                Ok = data.Ok,
                Status = data.Status,
                Data = data.Data as OutboxEmailAddress
            };

            // outbox
            // 判断是否可用
            if (data.Data is not OutboxEmailAddress outbox)
            {
                return new FuncResult<OutboxEmailAddress>()
                {
                    Ok = false,
                    Status = PoolResultStatus.EmptyError,
                    Message = $"未能从{UserId}池中获取发件箱"
                };
            }

            if (!outbox.LockUsing())
            {
                // 获取使用权失败
                return new FuncResult<OutboxEmailAddress>()
                {
                    Ok = false,
                    Status = PoolResultStatus.LockError,
                    Message = $"发件箱 {outbox.Email} 已被其它线程使用，锁定失败"
                };
            }

            // 保存当前引用
            scopeServices.UserOutboxesPool = this;
            return new FuncResult<OutboxEmailAddress>()
            {
                Data = outbox,
                Ok = true
            };
        }

        /// <summary>
        /// 邮件发送完成
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        public async Task EmailItemSendCompleted(SendingContext sendingContext)
        {
            // 移除发件箱
            if (sendingContext.OutboxEmailAddress.ShouldDispose)
            {
                _outboxes.TryRemove(sendingContext.OutboxEmailAddress.Email, out _);
                _logger.Info($"{sendingContext.OutboxEmailAddress.Email} 被标记为释放，从发件池中移除");
            }

            // 回调父级
            await sendingContext.UserOutboxesPoolManager.EmailItemSendCompleted(sendingContext);
        }

        /// <summary>
        /// 构建发送阶段
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        public async Task BuildSendingStage(ISendingContext sendingContext)
        {
            // 调用时肯定非空，因为有 enable 控制
            var outboxResult = _outboxes.GetDataByWeight();
            if (outboxResult.NotOk)
            {
                var sentResult = new SentResult(false, outboxResult.Message ?? $"未能从{UserId}池中获取发件箱")
                {
                    SentStatus = SentStatus.Failed
                };
                sendingContext.SetSendResult(sentResult);
                return;
            }

            // outbox
            // 判断是否可用
            if (outboxResult.Data is not OutboxEmailAddress outbox)
            {
                var result = new SentResult(false, "结果无法转成 OutboxEmailAddress")
                {
                    SentStatus = SentStatus.Failed
                };
                sendingContext.SetSendResult(result);
                return;
            }

            // 锁定邮箱
            if (!outbox.LockUsing())
            {
                // 获取使用权失败
                var result = new SentResult(false, $"发件箱 {outbox.Email} 已被其它线程使用，锁定失败")
                {
                    SentStatus = SentStatus.LockError | SentStatus.Failed
                };
                sendingContext.SetSendResult(result);
                return;
            }

            // 发件箱不再继续向下调用
        }

        public Task Execute(ISendingContext sendingContext)
        {
            throw new NotImplementedException();
        }

        public Task Dispose(ISendingContext sendingContext)
        {
            throw new NotImplementedException();
        }
    }
}
