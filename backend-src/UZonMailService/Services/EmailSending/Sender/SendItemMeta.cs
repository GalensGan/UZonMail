using UZonMailService.Models.SQL.EmailSending;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// SendItem的元数据
    /// </summary>
    public class SendItemMeta
    {
        public SendItemMeta(SendingItem sendingItem, string outboxEmail = "")
        {
            SendingItemId = sendingItem.Id;
            OutboxEmail = outboxEmail;
            TriedCount = sendingItem.TriedCount;
        }

        public long SendingItemId { get; set; }

        public string OutboxEmail { get; set; }

        public int TriedCount { get; set; }
    }
}
