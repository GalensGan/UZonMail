using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Database
{
   public interface IAccountDb
    {
        Account GetLatestVisitAccount();

        Account FindOneAccount(string userName);

        bool InsertAccount(Account account);

        bool UpdateAccount(Account account);
    }
}
