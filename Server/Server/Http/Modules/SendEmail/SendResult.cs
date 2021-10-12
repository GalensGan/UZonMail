using Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Modules.SendEmail
{
    /// <summary>
    /// 发送结果数据
    /// </summary>
    class SendResult
    {
        /// <summary>
        /// 是否发件成功
        /// </summary>
        public bool IsSent { get; set; }

        public SendItem SendItem { get; set; }

        public SendBox SendBox { get; set; }
    }
}
