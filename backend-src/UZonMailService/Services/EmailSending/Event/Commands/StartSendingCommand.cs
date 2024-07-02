using UZonMailService.Services.EmailSending.Pipeline;

namespace UZonMailService.Services.EmailSending.Event.Commands
{
    public class StartSendingCommand : GenericCommand<int>
    {
        public StartSendingCommand(SendingContext scopeServices, int count) : base(CommandType.StartSending, scopeServices, count)
        {
        }
    }
}
