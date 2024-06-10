using System.ComponentModel.DataAnnotations.Schema;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.UserInfos;

namespace UZonMailService.Models.SqlLite.Emails
{
    /// <summary>
    /// 收件箱
    /// </summary>
    public class Inbox : SqlId
    {
        public Inbox()
        {
            BoxType = EmailBoxType.Inbox;
        }

        /// <summary>
        /// 邮件组 id
        /// </summary>
        public int EmailGroupId { get; set; }

        // 冗余用户信息
        public int UserId { get; set; }

        private string _email = string.Empty;
        /// <summary>
        /// 收件箱
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                var index = value.LastIndexOf('@');
                // 提取邮箱类型
                if (index > 0)
                {
                    Domain = value[(index + 1)..];
                }
            }
        }

        /// <summary>
        /// 邮箱域名
        /// 方便统计
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 类型
        /// 由于 inbox 和 outbox 位于同一张表，所以需要区分
        /// </summary>
        public EmailBoxType BoxType { get; set; }

        /// <summary>
        /// 关联数
        /// 表示该邮件用于发件或收件的数量
        /// </summary>
        public int LinkCount { get; set; }
    }
}
