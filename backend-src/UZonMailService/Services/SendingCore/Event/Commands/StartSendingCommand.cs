using UZonMailService.Services.EmailSending.Pipeline;

namespace UZonMailService.Services.EmailSending.Event.Commands
{
    public class StartSendingCommand : GenericCommand<int>
    {
        public StartSendingCommand(int count, SendingContext? scopeServices = null) : base(CommandType.StartSending, scopeServices, count)
        {
        }
    }
}
