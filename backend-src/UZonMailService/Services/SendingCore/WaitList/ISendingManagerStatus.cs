namespace UZonMailService.Services.EmailSending.WaitList
{
    /// <summary>
    /// 验证是否为空
    /// </summary>
    public interface ISendingManagerStatus
    {
        /// <summary>
        /// 获取管理器状态
        /// 用户任务管理通过这个状态来判断是否需要释放
        /// </summary>
        /// <returns></returns>
        public SendingObjectStatus GetManagerStatus();

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="status"></param>
        public void ChangeStatus(SendingObjectStatus status);
    }
}
