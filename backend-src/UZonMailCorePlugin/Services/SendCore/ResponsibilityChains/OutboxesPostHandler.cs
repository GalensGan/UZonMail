using UZonMail.Core.Services.SendCore.Contexts;

namespace UZonMail.Core.Services.SendCore.ResponsibilityChains
{
    public class OutboxesPostHandler : AbstractSendingHandler
    {
        protected override Task HandleCore(SendingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
