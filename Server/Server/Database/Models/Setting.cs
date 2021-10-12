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
        
        /// <summary>
        /// 时间间隔最大值
        /// </summary>
        public double sendInterval_max { get; set; }

        /// <summary>
        /// 发送时间间隔最小值
        /// </summary>
        public double sendInterval_min { get; set; }

        // 是否自动发送
        public bool isAutoResend { get; set; }

        /// <summary>
        /// 图文混发
        /// </summary>
        public bool sendWithImageAndHtml { get; set; }

        // 单日最大发件量
        public int maxEmailsPerDay { get; set; }
    }
}
