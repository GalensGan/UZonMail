
namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 当主机使用 RemoteSender 发送邮件时，远程机通过该类发送邮件
    /// 本类与 LocalSender 的区别在于：本类发送后，会将结果返回给远程机
    /// </summary>
    public class SenderForRemote : LocalSender
    {
        private SendItem sendItem;
        public SenderForRemote(SendItem sendItem) : base(sendItem)
        {
            this.sendItem = sendItem;
        }

        public override Task<SentStatus> Send()
        {
            return UpdateSendingStatus(new SendCompleteResult(sendItem,true,""));
        }

        protected override async Task<SentStatus> UpdateSendingStatus(SendCompleteResult sendCompleteResult)
        {
            // 在此处将结果返回给远程机
            return SentStatus.OK;
        }
    }
}
