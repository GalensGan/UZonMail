using UZonMailService.SignalRHubs.Notify;
using UZonMailService.SignalRHubs.SendEmail;

namespace UZonMailService.SignalRHubs
{
    /// <summary>
    /// 客户端的方法
    /// </summary>
    public interface IUzonMailClient: ISendEmailClient, INotifyClient
    {
    }
}
