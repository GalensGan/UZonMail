using Uamazing.Utils.Results;

namespace UZonMailService.Services.EmailSending.Base
{
    public class FuncResult<T> : Result<T>
    {
        public PoolResultStatus Status { get; set; }
    }
}
