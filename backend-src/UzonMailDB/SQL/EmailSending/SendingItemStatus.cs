namespace UZonMail.DB.SQL.EmailSending
{
    /// <summary>
    /// 邮件状态
    /// </summary>
    [Flags]
    public enum SendingItemStatus
    {
        /// <summary>
        /// 发送失败
        /// </summary>
        Failed = 1 << 0,

        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 1 << 1,

        /// <summary>
        /// 已取消
        /// </summary>
        Cancel = 1 << 2,

        /// <summary>
        /// 初始状态
        /// </summary>
        Created = 1 << 3,

        /// <summary>
        /// 是否可以发送
        /// </summary>
        CanSend = 1 << 4,

        /// <summary>
        /// 等待发件中
        /// </summary>
        Pending = 1 << 5,

        /// <summary>
        /// 发送状态
        /// </summary>
        Sending = 1 << 6,

        /// <summary>
        /// 发送成功
        /// </summary>
        Success = 1 << 7,

        /// <summary>
        /// 已读
        /// </summary>
        Read = 1 << 8,

        /// <summary>
        /// 已取消订阅
        /// </summary>
        Unsubscribed = 1 << 9,
    }
}
