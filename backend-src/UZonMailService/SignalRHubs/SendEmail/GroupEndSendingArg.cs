using UZonMailService.Models.SqlLite.EmailSending;

namespace UZonMailService.SignalRHubs.SendEmail
{
    public class GroupEndSendingArg
    {
        public GroupEndSendingArg(SendingGroup sendingGroup, DateTime startDate)
        {
            StartDate = startDate;
            SendingGroupId = sendingGroup.Id;
            Total = sendingGroup.TotalCount;
            Success = sendingGroup.SuccessCount;
        }

        public DateTime StartDate { get; set; }
        public int SendingGroupId { get; set; }
        public int Total { get; set; }
        public int Success { get; set; }
        public string Message { get; set;}
    }
}
