using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class SendBox:EmailInfo
    {
        public string password { get; set; }
        public string smtp { get; set; }
    }
}
