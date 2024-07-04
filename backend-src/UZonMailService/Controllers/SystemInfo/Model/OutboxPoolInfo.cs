using UZonMailService.Services.EmailSending.OutboxPool;

namespace UZonMailService.Controllers.SystemInfo.Model
{
    public class OutboxPoolInfo
    {
        public long UserId { get; private set; }
        public int OutboxesCount { get; private set; }

        public OutboxPoolInfo(UserOutboxesPool userOutboxPool)
        {
            UserId = userOutboxPool.UserId;
            OutboxesCount = userOutboxPool.Count;
        }
    }
}
