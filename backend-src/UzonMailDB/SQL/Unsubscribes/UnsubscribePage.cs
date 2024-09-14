using UZonMail.DB.SQL.Base;

namespace UZonMail.DB.SQL.Unsubscribes
{
    /// <summary>
    /// 取消订阅的界面设置
    /// </summary>
    public class UnsubscribePage : OrgId
    {
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set;}

        /// <summary>
        /// Html 内容
        /// </summary>
        public string HtmlContent { get; set; }
    }
}
