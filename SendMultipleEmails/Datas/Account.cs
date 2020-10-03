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
        public string userName = string.Empty;

        // password 是加密过的
        public string passWord = string.Empty;

        public override bool Equals(object obj)
        {
            if(obj is Account account)
            {
                return account.userName.Equals(userName);
            }

            return false;
        }
        public override int GetHashCode()
        {
            return userName.GetHashCode();
        }
    }
}
