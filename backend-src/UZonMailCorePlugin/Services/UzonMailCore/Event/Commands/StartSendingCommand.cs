using UZonMail.Core.Services.EmailSending.Pipeline;

namespace UZonMail.Core.Services.EmailSending.Event.Commands
{
    public class StartSendingCommand : GenericCommand<int>
    {
        public StartSendingCommand(int count, SendingContext? scopeServices = null) : base(CommandType.StartSending, scopeServices, count)
        {
        }
    }
}
