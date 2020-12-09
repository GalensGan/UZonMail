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
    class LiteDbConcrete : IDatabase, IAccountDb, ISenderDb
    {
        private LiteDatabase _liteDb;
        public LiteDbConcrete(string dbPath)
        {
            _liteDb = new LiteDatabase(dbPath);
        }

        #region IAccount
        public ILiteCollection<Account> AccountC => _liteDb.GetCollection<Account>(DatabaseName.Account.ToString());
        public Account FindOneAccount(string userName)
        {
            return AccountC.FindOne(x => x.UserId == userName);
        }

        public Account GetLatestVisitAccount()
        {
            return AccountC.Query()
                .OrderByDescending(x => x.LastVisitTimestamp)
                .FirstOrDefault();
        }

        public bool InsertAccount(Account account)
        {
            return null != AccountC.Insert(account);
        }

        public bool UpdateAccount(Account account)
        {
            return AccountC.Update(new BsonDocument { [FieldKey.UserId.ToString()] = account.UserId }, account);
        }
        #endregion

        #region ISenders
        private ILiteCollection<Sender> SenderC => _liteDb.GetCollection<Sender>(DatabaseName.Sender.ToString());
        public bool InsertSender(Sender sender)
        {
            return null != SenderC.Insert(sender);
        }

        public bool InsertSenders(IEnumerable<Sender> senders)
        {
            return SenderC.InsertBulk(senders) > 0;
        }

        public bool DeleteSender(string senderId)
        {
            return SenderC.DeleteMany(s => s.Name == senderId) > 0;
        }

        public Sender FindOneSenderByName(string userId)
        {
            return SenderC.FindOne(s => s.Name == userId);
        }

        public Sender FindOneSenderByEmail(string email)
        {
            return SenderC.FindOne(s => s.Email == email);
        }

        public IEnumerable<Sender> FindAllSenders()
        {
            return SenderC.FindAll();
        }

        public bool UpdateSender(Sender sender) 
        {
            return SenderC.Update(new BsonDocument() { [FieldKey.Name.ToString()]=sender.Name },sender);
        }

        public int UpsertSenders(IEnumerable<Sender> objs)
        {
            return SenderC.Upsert(objs);
        }

        public bool DeleteSender(int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
