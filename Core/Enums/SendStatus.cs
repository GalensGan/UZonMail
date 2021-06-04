using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Enums
{
    public enum SendStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        New,

        /// <summary>
        /// 预览阶段
        /// </summary>
        Preview,

        /// <summary>
        /// 发送阶段
        /// </summary>
        Sending,

        /// <summary>
        /// 发送完成
        /// </summary>
        Sent
    }
}
