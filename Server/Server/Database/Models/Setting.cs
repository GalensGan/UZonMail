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
        /// <summary>
        /// 用户名
        /// </summary>
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

        /// <summary>
        /// 生成默认配置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Setting DefaultSetting(string userId)
        {
            return new Setting()
            {
                userId = userId,
                maxEmailsPerDay = 40,
                isAutoResend = true,
                sendInterval_max = 8,
                sendInterval_min = 3,
                sendWithImageAndHtml = false,
            };
        }
    }
}
