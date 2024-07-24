using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.Utils.UzonMail
{
    /// <summary>
    /// 发件箱地址
    /// </summary>
    public interface IOutboxEmailAddress
    {
        /// <summary>
        /// 地址类型
        /// </summary>
        OutboxEmailAddressType Type { get;}

        /// <summary>
        /// 用户 Id
        /// </summary>
        long UserId { get; }
    }
}
