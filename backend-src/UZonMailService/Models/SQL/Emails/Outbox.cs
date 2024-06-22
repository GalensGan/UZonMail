using UZonMailService.Models.SQL.EmailSending;
using UZonMailService.Services.EmailSending.OutboxPool;
using Uamazing.Utils.Extensions;
using UZonMailService.Models.SQL.Settings;

namespace UZonMailService.Models.SQL.Emails
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
        /// Smtp 的用户名
        /// 与 smtp 的发件邮箱可能不一致
        /// </summary>
        public string? UserName { get; set; }

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
        public long ProxyId { get; set; }

        /// <summary>
        /// 单日最大发送数量
        /// 为 0 时表示不限制
        /// </summary>
        public int MaxSendCountPerDay { get; set; }

        /// <summary>
        /// 当前已发送数量
        /// </summary>
        public int SentTotalToday { get; set; }

        /// <summary>
        /// 状态
        /// 用于向前端展示发件箱是否可用
        /// </summary>
        public OutboxStatus Status { get; set; }

        /// <summary>
        /// 转成发件地址
        /// </summary>
        /// <param name="userSetting"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public OutboxEmailAddress ToOutboxEmailAddress(UserSetting userSetting, long groupId, List<string> smtpPasswordSecretKeys)
        {
            // 对密码进行解密
            var plainPassword = Password.DeAES(smtpPasswordSecretKeys[0], smtpPasswordSecretKeys[1]);
            return new OutboxEmailAddress(userSetting)
            {
                // 对密码解密
                AuthPassword = plainPassword,
                AuthUserName = UserName,
                SmtpHost = SmtpHost,
                SmtpPort = SmtpPort,
                CreateDate = DateTime.Now,
                Email = Email,
                Name = Name,
                EnableSSL = EnableSSL,
                Id = Id,
                MaxSendCountPerDay = MaxSendCountPerDay,
                SentTotalToday = SentTotalToday,

                SendingGroupIds = [groupId],
                ProxyId = ProxyId
            };
        }
    }
}
