using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Modules.SendEmail
{
    class GenerateInfo
    {
        public bool ok;

        public int senderCount;

        /// <summary>
        /// 选择的收件人数量
        /// </summary>
        public int selectedReceiverCount;

        /// <summary>
        /// 数据中的收件人数据
        /// </summary>
        public int dataReceiverCount;

        /// <summary>
        /// 实际收件人数量
        /// </summary>
        public int acctualReceiverCount;

        public string message;

        public string historyId;
    }
}
