using UZonMailService.UzonMailDB.SQL.EmailSending;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// SendItem的元数据
    /// </summary>
    public class SendItemMeta
    {
        public SendItemMeta(long sendingItemId)
        {
            SendingItemId = sendingItemId;
        }

        public SendItemMeta(SendingItem sendingItem, string outboxEmail = "")
        {
            SendingItemId = sendingItem.Id;
            OutboxEmail = outboxEmail;
            _triedCount = sendingItem.TriedCount;
        }

        public long SendingItemId { get; private set; }

        public string OutboxEmail { get; set; }

        private int _triedCount;
        public int TriedCount => _triedCount;
        public void IncreaseTriedCount()
        {
           Interlocked.Increment(ref _triedCount);
        }

        public override int GetHashCode()
        {
            return SendingItemId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SendItemMeta meta)
            {
                return SendingItemId == meta.SendingItemId;
            }
            return false;
        }
    }
}
