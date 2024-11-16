using log4net;
using System.Collections.Concurrent;
using UZonMail.Core.Services.EmailSending.Base;
using UZonMail.Core.Services.EmailSending.Pipeline;
using UZonMail.Core.Services.EmailSending.Sender;
using UZonMail.Core.Services.SendCore.Outboxes;
using UZonMail.Core.Utils.Database;
using UZonMail.DB.SQL.EmailSending;

namespace UZonMail.Core.Services.EmailSending.WaitList
{
    /// <summary>
    /// 单个用户的发件任务管理
    /// 先添加先发送
    /// </summary>
    public class UserSendingGroupsPool : ConcurrentDictionary<long, SendingGroupTask>
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(UserSendingGroupsPool));

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userId"></param>
        public UserSendingGroupsPool(long userId)
        {
            UserId = userId;
        }

        #region 内部字段定义
        // 所有的发件箱
        private List<string> _outboxes = new();
        #endregion

        #region 公开属性

        #endregion

        /// <summary>
        /// 用户 id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 添加发件组任务
        /// 若包含 sendingItemIds，则只发送这部分邮件
        /// </summary>
        /// <param name="scopeServices"></param>
        /// <param name="sendingGroupId">传入时请保证组一定存在</param>
        /// <param name="smtpPasswordSecretKeys">smtp密码密钥</param>
        /// <param name="sendingItemIds">待发送的 Id</param>
        /// <returns></returns>
        public async Task<bool> AddSendingGroup(SendingContext scopeServices, long sendingGroupId, List<string> smtpPasswordSecretKeys, List<long>? sendingItemIds = null)
        {
            // 有可能发件组已经存在
            if (!this.TryGetValue(sendingGroupId, out var existTask))
            {
                // 重新初始化
                // 添加到列表
                var newTask = await SendingGroupTask.Create(scopeServices, sendingGroupId, smtpPasswordSecretKeys);
                if (newTask == null) return false;

                var success = await newTask.InitSendingItems(scopeServices, sendingItemIds);
                if (!success) return false;
                return this.TryAdd(sendingGroupId, newTask);
            }
            else
            {
                // 复用原来的数据
               return await existTask.InitSendingItems(scopeServices, sendingItemIds);
            }
        }

        /// <summary>
        /// 获取组中可被 outboxId 发送的邮件项
        /// </summary>
        /// <returns></returns>
        public async Task<SendItem?> GetSendItem(SendingContext scopeServices, OutboxEmailAddress outbox)
        {
            // 依次获取发件项
            foreach (var kv in this)
            {
                var groupTask = kv.Value;
                var sendItem = await groupTask.GetSendItem(scopeServices, outbox);
                if (sendItem != null)
                {
                    // 保存用户发件组池
                    scopeServices.UserSendingGroupsPool = this;
                    return sendItem;
                }
            }

            return null;
        }

        /// <summary>
        /// 邮件项发送完成
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        public async Task EmailItemSendCompleted(SendingContext sendingContext)
        {
            // 判断邮件任务是否已经发送完成
            if (sendingContext.SendingGroupTask.ShouldDispose)
            {
                // 说明已经发完了               
                // 移除当前任务
                await TryRemoveSendingGroupTask(sendingContext, sendingContext.SendingGroupTask.SendingGroupId);
                _logger.Info($"{sendingContext.SendingGroupTask.SendingGroupId} 可发邮件为空，从队列中移除");
            }

            // 向上回调
            await sendingContext.UserSendingGroupsManager.EmailItemSendCompleted(sendingContext);
        }

        public async Task<FuncResult<SendingGroupTask>> TryRemoveSendingGroupTask(SendingContext sendingContext,long sendingGroupId)
        {
            if (!this.TryRemove(sendingContext.SendingGroupTask.SendingGroupId, out var value))
                return new FuncResult<SendingGroupTask>()
                {
                    Ok = false,
                };

            // 更新数据库
            await sendingContext.SqlContext.SendingGroups.UpdateAsync(x => x.Id == sendingGroupId, x => x.SetProperty(y => y.Status, SendingGroupStatus.Finish));
            return new FuncResult<SendingGroupTask>()
            {
                Ok = true,
                Data = value,
            };
        }
    }
}
