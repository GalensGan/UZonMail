using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class SendBox : EmailInfo
    {
        public string password { get; set; }
        public string smtp { get; set; }

        /// <summary>
        /// 发件箱设置
        /// </summary>
        public SendBoxSetting settings { get; set; }

        /// <summary>
        /// 递增发件量
        /// </summary>
        /// <returns>true:可以继续发件；false:发件已达到最大数量</returns>
        public bool IncreaseSentCount(LiteDBManager liteDb,Setting globalSetting)
        {
            // 判断日期是否是今天，如果不是，则将当天发件数置 0
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            if (date != settings.recordDate)
            {
                settings.recordDate = date;
                settings.sentCountToday = 0;
            }

            settings.sentCountToday++;
            settings.sendCountTotal++;

            // 保存到数据库
            liteDb.Update(this);

            int maxEmails = settings.maxEmailsPerDay > 0 ? settings.maxEmailsPerDay : globalSetting.maxEmailsPerDay;

            if (maxEmails < 1) return true;

            return settings.sendCountTotal <= maxEmails;
        }
    }

    /// <summary>
    /// 发件箱设置
    /// </summary>
    public class SendBoxSetting
    {
        // 是否作为发件人
        public bool asSender { get; set; } = true;

        // 单日最大发件量
        public int maxEmailsPerDay { get; set; }

        // 总发件量
        // 系统自动增加
        public double sendCountTotal { get; set; }

        // 当天发件数
        public int sentCountToday { get; set; }

        // 记录单日发件的日期
        // 系统自动修改
        public string recordDate { get; set; }
    }
}
