using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Definitions
{
    [Flags]
    public enum SendStatus
    {
        /// <summary>
        /// 完成
        /// </summary>
        SendFinish = 1 << 0,

        /// <summary>
        /// 初始化
        /// </summary>
        Init = 1 << 1,

        /// <summary>
        /// 正在发送
        /// </summary>
        Sending = 1 << 2,

        /// <summary>
        /// 正在重发
        /// </summary>
        Resending = 1 << 3,

        /// <summary>
        /// 暂停
        /// </summary>
        Pause = 1 << 4,

        /// <summary>
        /// 图片发送
        /// </summary>
        AsImage = 1 << 5,

        /// <summary>
        /// html 发送
        /// </summary>
        AsHtml = 1 << 6,
    }
}
