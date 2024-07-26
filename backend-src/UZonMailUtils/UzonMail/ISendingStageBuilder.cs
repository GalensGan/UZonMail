using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UZonMail.Utils.UzonMail;

namespace Uamazing.Utils.UzonMail
{
    /// <summary>
    /// 生成发送阶段
    /// </summary>
    public interface ISendingStageBuilder
    {
        /// <summary>
        /// 生成发送阶段
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        Task BuildSendingStage(ISendingContext sendingContext);
    }
}
