using UZonMail.Core.Services.SendCore.Interfaces;
using UZonMail.Core.Services.SendCore.Outboxes;
using UZonMail.Utils.Web.Service;

namespace UZonMail.Core.Services.SendCore.Utils
{
    /// <summary>
    /// 释放用户资源
    /// </summary>
    public class UserResourceDispose(OutboxesPoolList outboxContainer) : ITransientService<IUserResourceDispose>, IUserResourceDispose
    {
        public void Dispose(long userId)
        {
            // 移除用户
        }
    }
}
