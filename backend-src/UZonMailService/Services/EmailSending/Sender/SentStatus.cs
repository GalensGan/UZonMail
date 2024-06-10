namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 发送状态
    /// </summary>
    public enum SentStatus
    {
        /// <summary>
        /// 成功
        /// </summary>
        OK,

        /// <summary>
        /// 需要重试
        /// </summary>
        Retry,

        /// <summary>
        /// 失败
        /// </summary>
        Failed
    }
}
