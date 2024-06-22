using UZonMailService.Models.SQL.Base;
using UZonMailService.Models.SQL.MultiTenant;
using Uamazing.Utils.Extensions;

namespace UZonMailService.Models.SQL.Emails
{
    /// <summary>
    /// 邮箱组
    /// 为了方便搜索和减少复杂度，使用扁平化的组，不使用树形结构
    /// </summary>
    public class EmailGroup:SqlId
    {
        public long UserId { get; set; }
        public User User { get; set; }

        /// <summary>
        /// 类型
        /// 1-发件箱
        /// 2-收件箱
        /// </summary>
        public EmailGroupType Type { get; set; }

        /// <summary>
        /// 图标名称
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// 父组 id
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public long Order { get; set; } = DateTime.Now.ToTimestamp();

        /// <summary>
        /// 收件箱
        /// </summary>
        public List<Inbox> Inboxes { get; set; }
    }
}
