using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UZonMailDesktop.Models
{
    internal interface IDetectRuntimeEnv
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get;}

        /// <summary>
        /// 检查环境
        /// </summary>
        bool DetectEnv();

        /// <summary>
        /// 失败后显示的消息
        /// </summary>
        string FailedMessage { get; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        string RedirectUrl { get; }
    }
}
