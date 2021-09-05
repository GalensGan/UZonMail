using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class Setting:AutoObjectId
    {
        public string userId { get; set; }
        
        public double sendInterval_max { get; set; }
        public double sendInterval_min { get; set; }

        // 是否自动发送
        public bool isAutoResend { get; set; }
    }
}
