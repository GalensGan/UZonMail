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
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        // password 是加密过的
        public string PassWord { get; set; } = string.Empty;

        // 上次访问时间
        public long LastVisitTimestamp { get; set; } = TimeHelper.TimestampNow();

        public override bool Equals(object obj)
        {
            if(obj is Account account)
            {
                return account.UserName.Equals(UserName);
            }

            return false;
        }
        public override int GetHashCode()
        {
            return UserName.GetHashCode();
        }
    }
}
