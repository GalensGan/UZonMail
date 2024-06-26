namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 发送状态
    /// </summary>
    [Flags]
    public enum SentStatus
    {
        /// <summary>
        /// 成功
        /// </summary>
        OK = 1,

        /// <summary>
        /// 需要重试
        /// </summary>
        Retry = 2,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 4,

        /// <summary>
        /// 发件箱连接错误
        /// </summary>
        OutboxConnectError = 8
    }
}
