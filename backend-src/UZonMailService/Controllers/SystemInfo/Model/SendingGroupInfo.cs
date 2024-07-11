using UZonMailService.Services.EmailSending.WaitList;

namespace UZonMailService.Controllers.SystemInfo.Model
{
    public class SendingGroupInfo
    {
        public long UserId { get; private set; }
        public int SendingGroupsCount { get; private set; }

        public SendingGroupInfo(UserSendingGroupsPool sendingGroupsPool)
        {
            UserId = sendingGroupsPool.UserId;
            SendingGroupsCount = sendingGroupsPool.Count;
        }
    }
}
