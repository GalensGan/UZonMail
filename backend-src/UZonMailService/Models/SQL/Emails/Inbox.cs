using System.ComponentModel.DataAnnotations.Schema;
using UZonMailService.Models.SQL.Base;
using UZonMailService.Models.SQL.MultiTenant;

namespace UZonMailService.Models.SQL.Emails
{
    /// <summary>
    /// 收件箱
    /// </summary>
    public class Inbox : EmailBox
    {
        public Inbox()
        {
            BoxType = EmailBoxType.Inbox;
        }

        /// <summary>
        /// 上一次成功发件的日期
        /// 同一个组织内，共用日期
        /// </summary>
        public DateTime LastSuccessDeliveryDate { get; set; }

        /// <summary>
        /// 上一次被发件日期
        /// </summary>
        public DateTime LastBeDeliveredDate { get; set; }
    }
}
