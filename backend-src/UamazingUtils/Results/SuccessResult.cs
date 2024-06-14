using Uamazing.Utils.Results;

namespace Uamazing.Utils.Results
{
    public class SuccessResult<T> : Result<T>
    {
        public SuccessResult(T data) : base(true, "success",data)
        {
        }
    }
}
