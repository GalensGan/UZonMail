
namespace UZonMail.Core.Services.EmailSending.Sender
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
        /// 构造函数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="tokenSource"></param>
        public EmailSendingTask(Action action, CancellationTokenSource tokenSource) : base(action, tokenSource.Token)
        {
            CancelTokenSource = tokenSource;
        }
    }
}
