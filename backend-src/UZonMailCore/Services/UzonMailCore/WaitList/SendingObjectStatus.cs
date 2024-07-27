namespace UZonMail.Core.Services.EmailSending.WaitList
{
    /// <summary>
    /// 发送对象的状态
    /// </summary>
    public enum SendingObjectStatus
    {
        /// <summary>
        /// 正常状态
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 挂起状态
        /// 1. 当里面没有收件箱，但有邮件正在发送时，进入挂起状态，保证邮件失败后，可以重发
        /// 2. 当所有发件箱处于冷却状态时，进入挂起状态，等待下次发送
        /// 3. 用户暂停了该次发件，今后对于暂停的发件，可以考虑直接删除
        /// </summary>
        Pending,

        /// <summary>
        /// 可以释放
        /// </summary>
        ShouldDispose,

        /// <summary>
        /// 已经释放
        /// </summary>
        Disposed,
    }
}
