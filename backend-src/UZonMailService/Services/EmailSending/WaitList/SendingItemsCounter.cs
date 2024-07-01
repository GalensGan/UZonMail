namespace UZonMailService.Services.EmailSending.WaitList
{
    public class SendingItemsCounter
    {
        #region 从数据库中读取的初始状态
        private readonly int initSuccessCount;
        private readonly int initSentCount;
        private readonly int initTotal;
        #endregion

        public SendingItemsCounter(int total,int sent,int success)
        {
            initTotal = total;
            initSentCount = sent;
            initSuccessCount = success;
        }

        private int _currentSuccessCount;
        public int CurrentSuccessCount => _currentSuccessCount;

        private int _currentSentCount;
        public int CurrentSentCount => _currentSuccessCount;

        private int _currentTotal;
        public int CurrentTotal => _currentTotal;

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
        public void IncreaseSuccessCount(bool success)
        {
            Interlocked.Increment(ref _currentSentCount);
            if (success) Interlocked.Increment(ref _currentSuccessCount);
        }

        /// <summary>
        /// 总成功数
        /// </summary>
        public int TotalSuccessCount => initSuccessCount + CurrentSuccessCount;

        /// <summary>
        /// 总发送数
        /// </summary>
        public int TotalSentCount => initSentCount + CurrentSentCount;
    }
}
