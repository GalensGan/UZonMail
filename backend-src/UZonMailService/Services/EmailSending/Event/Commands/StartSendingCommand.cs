using UZonMailService.Services.EmailSending.Models;

namespace UZonMailService.Services.EmailSending.Event.Commands
{
    public class StartSendingCommand : GenericCommand<int>
    {
        public StartSendingCommand(ScopeServices scopeServices, int count) : base(CommandType.StartSending, scopeServices, count)
        {
        }
    }
}
