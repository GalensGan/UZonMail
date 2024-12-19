using UZonMail.Core.Services.SendCore.Interfaces;
using UZonMail.Utils.Web.Service;

namespace UZonMail.Core.Services.SendCore.Outboxes
{
    public class OutboxDispose : ITransientService<IOutboxDispose>, IOutboxDispose
    {
        public void Dispose()
        {
            
        }
    }
}
