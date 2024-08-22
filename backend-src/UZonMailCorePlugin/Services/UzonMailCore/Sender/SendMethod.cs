using UZonMail.Core.Services.EmailSending.Pipeline;

namespace UZonMail.Core.Services.EmailSending.Sender
{
    /// <summary>
    /// 发件基类
    /// </summary>
    public abstract class SendMethod
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns></returns>
        public abstract Task Send(SendingContext sendingContext);
    }
}
