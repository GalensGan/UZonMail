using UZonMail.Utils.Results;

namespace UZonMail.Core.Services.EmailSending.Base
{
    public class FuncResult<T> : Result<T>
    {
        public PoolResultStatus Status { get; set; }
    }
}
