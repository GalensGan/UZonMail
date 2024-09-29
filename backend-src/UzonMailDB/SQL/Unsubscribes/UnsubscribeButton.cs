using UZonMail.DB.SQL.Base;

namespace UZonMail.DB.SQL.Unsubscribes
{
    /// <summary>
    /// 退订按钮样式
    /// </summary>
    public class UnsubscribeButton : OrgId
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 退定按钮 Html
        /// 行内样式
        /// </summary>
        public string ButtonHtml { get; set; }

        /// <summary>
        /// 用户 Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 是否共享
        /// 共享后，其它同组织用户可以使用
        /// </summary>
        public bool IsShared { get; set; }
    }
}
