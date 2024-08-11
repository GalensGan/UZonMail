using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UZonMail.DB.SQL.Base;

namespace UZonMail.DB.SQL.License
{
    /// <summary>
    /// 授权信息
    /// </summary>
    public class LicenseInfo : SqlId
    {
        /// <summary>
        /// 授权码
        /// </summary>
        public string LicenseKey { get; set; }

        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime ActiveDate { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// 最近更新日期
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// 授权类型
        /// </summary>
        public LicenseType LicenseType { get; set; }
    }
}
