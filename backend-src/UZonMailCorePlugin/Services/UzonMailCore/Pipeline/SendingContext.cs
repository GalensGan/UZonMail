using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using UZonMail.Core.Services.EmailSending.OutboxPool;
using UZonMail.Core.Services.EmailSending.Sender;
using UZonMail.Core.Services.SendCore.Outboxes;
using UZonMail.Core.Services.SendCore.WaitList;
using UZonMail.Core.SignalRHubs;
using UZonMail.DB.SQL;

namespace UZonMail.Core.Services.EmailSending.Pipeline
{
    /// <summary>
    /// 发送上下文
    /// 每一次发送任务都会创建一个新的上下文
    /// </summary>
    public class SendingContext
    {
        public SendingContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            SqlContext = serviceProvider.GetRequiredService<SqlContext>();
        }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public SqlContext SqlContext { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// 获取 SignalR 客户端
        /// </summary>
        public IHubContext<UzonMailHub, IUzonMailClient> HubClient => ServiceProvider.GetRequiredService<IHubContext<UzonMailHub, IUzonMailClient>>();

        #region 发件池中间变量
        /// <summary>
        /// 所有用户的发件箱池管理器
        /// </summary>
        public UserOutboxesPoolManager? UserOutboxesPoolManager { get; set; }

        /// <summary>
        /// 用户发件箱池
        /// </summary>
        public OutboxesPool? UserOutboxesPool { get; set; }

        /// <summary>
        /// 发件箱
        /// </summary>
        public OutboxEmailAddress? OutboxEmailAddress { get; set; }
        #endregion

        #region 邮件队列相关变量
        /// <summary>
        /// 所有用户的发件组管理器
        /// </summary>
        public GroupTasksList? UserSendingGroupsManager { get; set; }

        /// <summary>
        /// 用户发件池队列
        /// </summary>
        public GroupTasks? UserSendingGroupsPool { get; set; }

        /// <summary>
        /// 发送任务
        /// </summary>
        public SendingGroupTask? SendingGroupTask { get; set; }

        /// <summary>
        /// 发送项
        /// </summary>
        public EmailItem? SendItem { get; set; }
        #endregion

        #region 发送结果
        public SendResult? SendResult { get; set; }

        /// <summary>
        /// 设置发送结果
        /// </summary>
        /// <param name="result"></param>
        public void SetSendResult(SendResult result)
        {
            if (SendResult != null) throw new InvalidOperationException("发送结果无法多次设置");
            SendResult = result;
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public async Task DisposeAsync()
        {
            if (OutboxEmailAddress != null)
            {
                await OutboxEmailAddress.StartCooling(this);
                OutboxEmailAddress.UnlockUsing();
            }

            // 清空数据
            UserOutboxesPoolManager = null;
            UserOutboxesPool = null;
            OutboxEmailAddress = null;
            UserSendingGroupsManager = null;
            UserSendingGroupsPool = null;
            SendingGroupTask = null;
            SendItem = null;
            SendResult = null;
        }
    }
}
