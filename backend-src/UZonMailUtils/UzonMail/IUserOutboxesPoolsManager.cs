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
        Task EmailItemSendCompleted(ISendingContext sendingContext);
    }
}
