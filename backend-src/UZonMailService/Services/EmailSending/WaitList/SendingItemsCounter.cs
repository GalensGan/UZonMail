namespace UZonMailService.Services.EmailSending.WaitList
{
    public class SendingItemsCounter
    {
        private readonly int initSuccessCount;
        private readonly int initSentCount;
        private readonly int initTotal;

        public SendingItemsCounter(int total,int sent,int success)
        {
            initTotal = total;
            initSentCount = sent;
            initSuccessCount = success;
        }

        public int CurrentSuccessCount { get; private set; }
        public int CurrentSentCount { get; private set; }
        public int CurrentTotal { get; set; } = 0;

        public void IncreaseSuccessCount(bool success)
        {
            CurrentSentCount++;
            if (success) CurrentSuccessCount++;
        }

        public int TotalSuccessCount => initSuccessCount + CurrentSuccessCount;
        public int TotalSentCount => initSentCount + CurrentSentCount;
    }
}
