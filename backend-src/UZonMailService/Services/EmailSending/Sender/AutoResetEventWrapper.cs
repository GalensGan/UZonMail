namespace UZonMailService.Services.EmailSending.Sender
{
    public class AutoResetEventWrapper
    {
        private AutoResetEvent _autoResetEvent;

        /// <summary>
        /// 是否处于等待状态
        /// </summary>
        public bool IsWaiting { get; private set; } = false;

        /// <summary>
        /// 初始化
        /// </summary>
        public AutoResetEventWrapper(bool initialState)
        {
            IsWaiting = initialState;
            _autoResetEvent = new AutoResetEvent(IsWaiting);
        }

        /// <summary>
        /// 使线程继续
        /// </summary>
        public void Set()
        {
            IsWaiting = false;
            _autoResetEvent.Set();
        }

        /// <summary>
        /// 使线程等待
        /// </summary>
        public void Reset()
        {
            IsWaiting = true;
            _autoResetEvent.Reset();
        }

        /// <summary>
        /// 等待信号
        /// </summary>
        public void WaitOne()
        {
            IsWaiting = true;
            _autoResetEvent.WaitOne();
        }
    }
}
