using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Modules.SendEmail
{
    public class SendingProgressInfo
    {
        public string historyId { get; set; }
        public int index { get; set; } = 0;
        public int total { get; set; } = 1;
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string receiverName { get; set; }
        public string receiverEmail { get; set; }
    }
}
