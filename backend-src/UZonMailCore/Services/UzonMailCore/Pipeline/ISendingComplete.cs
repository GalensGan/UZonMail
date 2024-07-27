using System.Threading.Tasks;

namespace UZonMail.Core.Services.EmailSending.Pipeline
{
    public interface ISendingComplete
    {
        Task EmailItemSendCompleted(SendingContext sendingContext);
    }
}
