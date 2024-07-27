using UZonMail.Core.Services.EmailSending.WaitList;

namespace UZonMail.Core.Controllers.SystemInfo.Model
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
