using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Database
{
   public interface IAccount
    {
        Account GetLatestVisitAccount();

        Account FindAccount(string userName);

        bool InsertAccount(Account account);

        bool UpdateAccount(Account account);
    }
}
