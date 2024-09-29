using UZonMail.DB.SQL.Base;

namespace UZonMail.DB.SQL.Unsubscribes
{
    /// <summary>
    /// 退订邮件
    /// </summary>
    public class UnsubscribeEmail : OrgId
    {
        /// <summary>
        /// 退订邮箱
        /// </summary>
        public string Email { get; set; }
    }
}
