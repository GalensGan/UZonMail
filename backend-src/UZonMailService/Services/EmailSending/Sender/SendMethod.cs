namespace UZonMailService.Services.EmailSending.Sender
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
        public abstract Task<SendResult> Send();
    }
}
