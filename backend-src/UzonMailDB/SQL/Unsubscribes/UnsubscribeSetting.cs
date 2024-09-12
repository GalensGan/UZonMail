using UZonMail.DB.SQL.Base;

namespace UZonMail.DB.SQL.Unsubscribes
{
    /// <summary>
    /// 退定设置
    /// </summary>
    public class UnsubscribeSetting : OrgId
    {
        /// <summary>
        /// 是否启用退订
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public UnsubscibeType Type { get; set; }

        /// <summary>
        /// 外部 URL
        /// </summary>
        public string? ExternalUrl { get; set; }
    }
}
