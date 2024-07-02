namespace UZonMailService.Services.EmailSending.Pipeline
{
    public interface ISendingComplete
    {
        Task EmailItemSendCompleted(SendingContext sendingContext);
    }
}
