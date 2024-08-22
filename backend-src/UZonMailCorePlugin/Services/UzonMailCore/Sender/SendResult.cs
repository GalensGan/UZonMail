namespace UZonMail.Core.Services.EmailSending.Sender
{
    /// <summary>
    /// 发送完成的结果
    /// </summary>
    public class SendResult
    {
        public SendResult(bool ok, string message)
        {
            Ok = ok;
            Message = message;
        }

        /// <summary>
        /// 是否ok
        /// </summary>
        public bool Ok { get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 发送状态
        /// </summary>
        public SentStatus SentStatus { get; set; } = SentStatus.OK;
    }
}
