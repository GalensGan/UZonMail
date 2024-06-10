namespace Uamazing.Utils.Web.Token
{
    /// <summary>
    /// 创建 token 的参数
    /// </summary>
    public class TokenParams
    {
        /// <summary>
        /// 过期时间
        /// ms
        /// </summary>
        public long Expire { get; set; }

        /// <summary>
        /// token 的密钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 签发者
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// 接收者
        /// </summary>
        public string Audience { get; set; } = string.Empty;
    }
}
