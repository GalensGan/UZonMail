using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UZonMail.DB.SQL.License
{
    /// <summary>
    /// 类型
    /// </summary>
    public enum LicenseType
    {
        None,

        /// <summary>
        /// 社区版本，免费
        /// </summary>
        Community,

        /// <summary>
        /// 专业版
        /// </summary>
        Professional,

        /// <summary>
        /// 企业版
        /// </summary>
        Enterprise,
    }
}
