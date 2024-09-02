namespace UZonMail.Core.Services.EmailSending.Sender
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
        OK = 1 << 0,

        /// <summary>
        /// 重试
        /// </summary>
        Retry = 1 << 1,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 1 << 2,

        /// <summary>
        /// 发件箱错误
        /// </summary>
        OutboxError = 1 << 3,

        /// <summary>
        /// 空发件组
        /// </summary>
        EmptySendingGroup = 1 << 4,

        /// <summary>
        /// 禁止重试
        /// </summary>
        ForbiddenRetring = 1 << 5
    }
}
