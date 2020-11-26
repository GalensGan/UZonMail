using LiteDB;
using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Database
{
    class LiteDbConcrete : IDatabase, IAccount
    {
        private LiteDatabase _accountLiteDb;
        public LiteDbConcrete(string dbPath)
        {
            _accountLiteDb = new LiteDatabase(dbPath);
        }

        #region IAccount
        public Account FindAccount(string userName)
        {
            return _accountLiteDb.GetCollection<Account>(DatabaseName.Account.ToString()).FindOne(x => x.UserId == userName);
        }

        public Account GetLatestVisitAccount()
        {
            return _accountLiteDb.GetCollection<Account>(DatabaseName.Account.ToString()).Query()
                .OrderByDescending(x => x.LastVisitTimestamp)
                .FirstOrDefault();
        }

        public bool InsertAccount(Account account)
        {
            return null != _accountLiteDb.GetCollection<Account>(DatabaseName.Account.ToString()).Insert(account);
        }

        public bool UpdateAccount(Account account)
        {
            return _accountLiteDb.GetCollection<Account>(DatabaseName.Account.ToString()).Update(account);
        }
        #endregion

    }
}
