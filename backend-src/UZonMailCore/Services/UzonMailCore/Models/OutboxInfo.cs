namespace UZonMail.Core.Services.EmailSending.Models
{
    /// <summary>
    /// 发件箱信息
    /// </summary>
    public class OutboxInfo
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string SmtpHost { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int SmtpPort { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 代理
        /// </summary>
        public string? ProxyHost { get; set; }

        /// <summary>
        /// 代理端口
        /// </summary>
        public int ProxyPort { get; set; }
    }
}
