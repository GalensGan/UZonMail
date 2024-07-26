using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UZonMail.Utils.UzonMail
{
    /// <summary>
    /// 发送阶段
    /// </summary>
    public interface ISendingStage
    {
        Task Execute(ISendingContext sendingContext);

        /// <summary>
        /// 是否应该释放
        /// </summary>
        bool ShouldDispose { get; }

        /// <summary>
        /// 阶段名称
        /// 可以通过这个名称来查找阶段
        /// </summary>
        string StageName { get; }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        Task Dispose(ISendingContext sendingContext);
    }
}
