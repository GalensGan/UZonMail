using UZonMail.Core.Services.SendCore.Contexts;
using UZonMail.Utils.Web.Service;

namespace UZonMail.Core.Services.SendCore.Actions
{
    /// <summary>
    /// 移除发件项
    /// </summary>
    public class RemoveSendingItemAction : IAction, ITransientService
    {
        /// <summary>
        /// 是否已激活
        /// </summary>
        public bool Active { get;private set; }

        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Run(SendingContext context)
        {
            if (Active) return;
        }
    }
}
