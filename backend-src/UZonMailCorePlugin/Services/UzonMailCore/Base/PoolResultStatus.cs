namespace UZonMail.Core.Services.EmailSending.Base
{
    public enum PoolResultStatus
    {
        None,

        WeightZeroError,

        EmptyError,

        CooldownError,

        LockError
    }
}
