using UZonMailService.Services.EmailSending.Sender;

namespace UZonMailService.Services.EmailSending.WaitList
{
    /// <summary>
    /// 待发件列表
    /// </summary>
    public interface ISendingWaitList
    {
        /// <summary>
        /// 获取发件箱的数量
        /// 若数量为 0，发送模块会被释放，可以通过这个特性来控制发送模块的生命周期
        /// </summary>
        /// <returns></returns>
        int GetOutboxesCount();

        /// <summary>
        /// 获取一个发件项
        /// 若没有发件项，返回 null
        /// 返回 null 时，发送模块会暂停
        /// </summary>
        /// <returns></returns>
        SendItem? GetSendItem();
    }
}
