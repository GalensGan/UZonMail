using Microsoft.AspNetCore.SignalR;
using UZonMail.Utils.UzonMail;
using UZonMailService.UzonMailDB.SQL;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.EmailSending.WaitList;
using UZonMailService.SignalRHubs;

namespace UZonMailService.Services.EmailSending.Pipeline
{
    /// <summary>
    /// 发送上下文
    /// 每一次发送任务都会创建一个新的上下文
    /// </summary>
    public class SendingContext : ISendingContext
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
        /// Service Provider
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// 获取 SignalR 客户端
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public IHubContext<THub,TInterface> GetHubClient<THub, TInterface>() where THub : Hub<TInterface> where TInterface : class
        {
            return ServiceProvider.GetRequiredService<IHubContext<THub, TInterface>>();
        }

        /// <summary>
        /// 获取 SignalR 客户端
        /// </summary>
        [Obsolete("新版本弃用")]
        public IHubContext<UzonMailHub, IUzonMailClient> HubClient => ServiceProvider.GetRequiredService<IHubContext<UzonMailHub, IUzonMailClient>>();

        #region 发件池中间变量
        /// <summary>
        /// 所有用户的发件箱池管理器
        /// </summary>
        [Obsolete("新版本弃用")]
        public UserOutboxesPoolsManager? UserOutboxesPoolManager { get; set; }

        /// <summary>
        /// 用户发件箱池
        /// </summary>
        public UserOutboxesPool? UserOutboxesPool { get; set; }

        /// <summary>
        /// 发件箱
        /// </summary>
        public OutboxEmailAddress? OutboxEmailAddress { get; set; }
        #endregion

        #region 邮件队列相关变量
        /// <summary>
        /// 所有用户的发件组管理器
        /// </summary>
        public UserSendingGroupsManager? UserSendingGroupsManager { get; set; }

        /// <summary>
        /// 用户发件池队列
        /// </summary>
        public UserSendingGroupsPool? UserSendingGroupsPool { get; set; }

        /// <summary>
        /// 发送任务
        /// </summary>
        public SendingGroupTask? SendingGroupTask { get; set; }

        /// <summary>
        /// 发送项
        /// </summary>
        public SendItem? SendItem { get; set; }
        #endregion

        /// <summary>
        /// 发送阶段
        /// 这是一个洋葱模型，先从上向下开始遍历，然后从下向上执行
        /// </summary>
        public List<ISendingStage> SendingStages { get; } = [];

        #region 发送结果
        public SentResult? SendResult { get; set; }

        /// <summary>
        /// 设置发送结果
        /// </summary>
        /// <param name="result"></param>
        public void SetSendResult(SentResult result)
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
                await OutboxEmailAddress.SetCooldown(this);
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
