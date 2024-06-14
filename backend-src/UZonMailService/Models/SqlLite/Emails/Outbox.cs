using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Services.EmailSending.OutboxPool;
using Uamazing.Utils.Extensions;
using UZonMailService.Models.SqlLite.Settings;

namespace UZonMailService.Models.SqlLite.Emails
{
    /// <summary>
    /// 发件箱
    /// </summary>
    public class Outbox : Inbox
    {
        public Outbox()
        {
            BoxType = EmailBoxType.Outbox;
        }

        /// <summary>
        /// SMTP 服务器地址
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// smtp 密码，需要加密保存
        /// smpt 密码 = 原始密码  > aes (sha256 作为 key)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否启用 SSL
        /// </summary>
        public bool EnableSSL { get; set; } = true;

        /// <summary>
        /// 代理 Id
        /// </summary>
        public int ProxyId { get; set; }

        /// <summary>
        /// 单日最大发送数量
        /// 为 0 时表示不限制
        /// </summary>
        public int MaxSendCountPerDay { get; set; }

        /// <summary>
        /// 转成发件地址
        /// </summary>
        /// <param name="userSetting"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public OutboxEmailAddress ToOutboxEmailAddress(UserSetting userSetting, int groupId,List<string> smtpPasswordSecretKeys)
        {
            // 对密码进行解密
            var plainPassword = Password.DeAES(smtpPasswordSecretKeys[0], smtpPasswordSecretKeys[1]);
            return new OutboxEmailAddress(userSetting)
            {
                // 对密码解密
                AuthPassword = plainPassword,
                AuthUserName = Email,
                SmtpHost = SmtpHost,
                SmtpPort = SmtpPort,
                CreateDate = DateTime.Now,
                Email = Email,
                Name = Name,
                EnableSSL = EnableSSL,
                Id = Id,
                SendingGroupIds = [groupId],
                ProxyId = ProxyId
            };
        }
    }
}
