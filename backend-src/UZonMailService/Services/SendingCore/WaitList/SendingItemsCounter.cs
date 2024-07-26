namespace UZonMailService.Services.EmailSending.WaitList
{
    public class SendingItemsCounter
    {
        #region 从数据库中读取的初始状态
        public int InitSuccessCount { get; }
        public  int InitSentCount { get; }
        public  int InitTotal { get; }
        #endregion

        public SendingItemsCounter(int total, int sent, int success)
        {
            InitTotal = total;
            InitSentCount = sent;
            InitSuccessCount = success;
        }

        private int _currentSuccessCount;
        public int CurrentSuccessCount => _currentSuccessCount;

        private int _currentSentCount;
        public int CurrentSentCount => _currentSentCount;

        private int _currentTotal;
        public int CurrentTotal => _currentTotal;

        private int _runningCount;
        public int RunningCount => _runningCount;

        /// <summary>
        /// 添加发送的数量
        /// </summary>
        /// <param name="count"></param>
        public void IncreaseTotalCount(int count)
        {
            Interlocked.Add(ref _currentTotal, count);
        }

        /// <summary>
        /// 增加发送完成的数量
        /// </summary>
        /// <param name="success"></param>
        public void IncreaseSentCount(bool success)
        {
            Interlocked.Increment(ref _currentSentCount);
            if (success) Interlocked.Increment(ref _currentSuccessCount);
        }

        /// <summary>
        /// 增加执行数量
        /// </summary>
        /// <param name="count"></param>
        public void IncreaseRunningCount(int count)
        {
            Interlocked.Add(ref _runningCount, count);
        }

        /// <summary>
        /// 总成功数
        /// </summary>
        public int TotalSuccessCount => InitSuccessCount + CurrentSuccessCount;

        /// <summary>
        /// 总发送数
        /// </summary>
        public int TotalSentCount => InitSentCount + CurrentSentCount;
    }
}
