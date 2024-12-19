using UZonMail.Core.Services.SendCore.Contexts;

namespace UZonMail.Core.Services.SendCore.Actions
{
    public interface IAction
    {
        /// <summary>
        /// 是否已经激活过
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// 开始执行动作
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Run(SendingContext context);
    }
}
