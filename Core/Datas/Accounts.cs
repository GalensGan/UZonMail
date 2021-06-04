using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    /// <summary>
    /// 账户类
    /// </summary>
    public class Accounts
    {
        /// <summary>
        /// 所有用户
        /// </summary>
        public List<Account> accounts;

        /// <summary>
        /// 上一次登陆的用户
        /// </summary>
        public Account lastAccount;
    }
}
