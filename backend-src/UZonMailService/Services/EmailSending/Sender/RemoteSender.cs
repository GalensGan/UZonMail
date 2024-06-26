
namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 远程发件
    /// </summary>
    public class RemoteSender : SendMethod
    {
        public override Task<SendResult> Send()
        {
            throw new NotImplementedException();
        }
    }
}
