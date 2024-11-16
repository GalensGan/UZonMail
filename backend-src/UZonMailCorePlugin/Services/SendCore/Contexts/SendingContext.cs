using UZonMail.Core.Services.SendCore.Outboxes;
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
        /// 发件箱地址
        /// </summary>
        public OutboxEmailAddress? OutboxAddress { get; set; }
    }
}
