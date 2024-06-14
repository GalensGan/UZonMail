
namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 自定义的 Task
    /// </summary>
    public class EmailSendingTask : Task
    {
        /// <summary>
        /// 取消标记
        /// </summary>
        public readonly CancellationTokenSource CancelTokenSource;

        /// <summary>
        /// 线程信号
        /// </summary>
        public AutoResetEventWrapper AutoResetEventWrapper { get; set; }

        /// <summary>
        /// 是否处于等待状态
        /// </summary>
        public bool IsPending { get; set; }

        public EmailSendingTask(Action action, CancellationTokenSource tokenSource) : base(action, tokenSource.Token)
        {
            CancelTokenSource = tokenSource;
        }
    }
}
