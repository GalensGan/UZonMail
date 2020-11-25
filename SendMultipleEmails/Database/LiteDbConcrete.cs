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
        private LiteDatabase _liteDb;
        public LiteDbConcrete(string dbPath)
        {
            _liteDb = new LiteDatabase(dbPath);
        }

        #region IAccount
        public Account FindAccount(string userName)
        {
            return _liteDb.GetCollection<Account>(DatabaseName.Account.ToString()).FindOne(x => x.UserName == userName);
        }

        public Account GetLatestVisitAccount()
        {
            return _liteDb.GetCollection<Account>(DatabaseName.Account.ToString()).Query()
                .OrderByDescending(x => x.LastVisitTimestamp)
                .FirstOrDefault();
        }

        public bool InsertAccount(Account account)
        {
            return null != _liteDb.GetCollection<Account>(DatabaseName.Account.ToString()).Insert(account);
        }

        public bool UpdateAccount(Account account)
        {
            return _liteDb.GetCollection<Account>(DatabaseName.Account.ToString()).Update(account);
        }
        #endregion

    }
}
