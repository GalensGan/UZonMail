using GalensSDK.TimeEx;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    public class Account
    {
        public string UserId { get; set; } = string.Empty;

        // password 是加密过的
        public string PassWord { get; set; } = string.Empty;

        // 上次访问时间
        public long LastVisitTimestamp { get; set; } = TimeHelper.TimestampNow();

        /// <summary>
        /// 是否自动保存
        /// </summary>
        public bool IsAutoSave { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is Account account)
            {
                return account.UserId.Equals(UserId);
            }

            return false;
        }
        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }
    }
}
