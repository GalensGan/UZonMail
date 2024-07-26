using UZonMail.Utils.Results;
using UZonMailService.UzonMailDB.SQL;

namespace UZonMail.Utils.UzonMail
{
    /// <summary>
    /// 发送完成的结果
    /// </summary>
    public class SentResult(bool ok, string message) : Result<SentStatus>(ok, message)
    {
        /// <summary>
        /// 发送状态
        /// </summary>
        public SentStatus SentStatus { get; set; }
    }
}
