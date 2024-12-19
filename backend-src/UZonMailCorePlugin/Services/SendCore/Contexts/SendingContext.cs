using Microsoft.AspNetCore.SignalR;
using UZonMail.Core.Services.SendCore.Actions;
using UZonMail.Core.Services.SendCore.Outboxes;
using UZonMail.Core.Services.SendCore.WaitList;
using UZonMail.Core.SignalRHubs;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.Utils.Web.Service;

namespace UZonMail.Core.Services.SendCore.Contexts
{
    public class SendingContext(IServiceProvider provider) : ITransientService
    {
        public IServiceProvider Provider => provider;

        /// <summary>
        /// 上下文状态
        /// </summary>
        public ContextStatus Status { get; set; }

        /// <summary>
        /// 获取 SignalR 客户端
        /// </summary>
        public IHubContext<UzonMailHub, IUzonMailClient> HubClient => Provider.GetRequiredService<IHubContext<UzonMailHub, IUzonMailClient>>();

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public SqlContext SqlContext => Provider.GetRequiredService<SqlContext>();

        #region 中间变量
        /// <summary>
        /// 发件箱地址
        /// </summary>
        public OutboxEmailAddress? OutboxAddress { get; set; }

        /// <summary>
        /// 发件项
        /// </summary>
        public SendItemMeta? EmailItem { get; set; }

        /// <summary>
        /// 执行的动作
        /// 在这个动作里，进行数据清除等操作
        /// </summary>
        public List<IAction> Actions => [];
        #endregion
    }
}
