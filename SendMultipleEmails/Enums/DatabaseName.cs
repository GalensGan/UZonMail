using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Enums
{
    public enum DatabaseName
    {
        /// <summary>
        /// 账户表
        /// </summary>
        Account,

        /// <summary>
        /// 发件人
        /// </summary>
        Senders,

        /// <summary>
        /// 收件人
        /// </summary>
        Receivers,

        /// <summary>
        /// 发送历史
        /// </summary>
        SendHistory,
    }
}
