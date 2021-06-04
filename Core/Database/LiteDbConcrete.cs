using LiteDB;
using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Database
{
    class LiteDbConcrete : IDatabase, IAccountDb, ISenderDb,IReceiverDb,IGroup
    {
        private LiteDatabase _liteDb;
        public LiteDbConcrete(string dbPath)
        {
            // 判断父目录是否存在，如果不存在，则要新建目录
            string dir = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
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
            return SenderC.DeleteMany(s => s.UserId == senderId) > 0;
        }

        public Sender FindById(int id)
        {
            return SenderC.FindById(new BsonValue(id));
        }

        public Sender FindOneSenderByUserId(string userId)
        {
            return SenderC.FindOne(s => s.UserId == userId);
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
            return SenderC.Update(sender);
        }

        public int UpsertSenders(IEnumerable<Sender> objs)
        {
            return SenderC.Upsert(objs);
        }

        public bool DeleteSender(int id)
        {
            return SenderC.Delete(new BsonValue(id));
        }
        #endregion

        #region IReceiver
        private ILiteCollection<Receiver> ReceiverC => _liteDb.GetCollection<Receiver>(DatabaseName.Receiver.ToString());
        public bool InsertReceiver(Receiver obj)
        {
            return null != ReceiverC.Insert(obj);
        }

        public bool InsertReceivers(IEnumerable<Receiver> objs)
        {
            return ReceiverC.InsertBulk(objs) > 0;
        }

        public bool DeleteReceiver(int id)
        {
            return ReceiverC.Delete(new BsonValue(id));
        }

        public bool DeleteAllReceivers()
        {
            return ReceiverC.DeleteAll() > 0;
        }

        public Receiver FindOneReceiverByUserId(string userId)
        {
            return ReceiverC.FindOne(item => item.UserId == userId);
        }

        public Receiver FindOneReceiverByEmail(string email)
        {
            return ReceiverC.FindOne(item => item.Email == email);
        }

        public IEnumerable<Receiver> FindAllReceivers()
        {
            return ReceiverC.FindAll();
        }

        public bool UpdateReceiver(Receiver obj)
        {
            return ReceiverC.Update(obj);
        }

        public int UpsertReceivers(IEnumerable<Receiver> objs)
        {
            return ReceiverC.Upsert(objs);
        }

        public IEnumerable<Receiver> FindReceiversByGroup(int groupId)
        {
            return ReceiverC.Find(item => item.GroupId == groupId);
        }
        #endregion

        #region IGroup
        private ILiteCollection<Group> GroupC => _liteDb.GetCollection<Group>(DatabaseName.Group.ToString());
        public IEnumerable<Group> GetAllGroups()
        {
            return this.GroupC.FindAll();
        }

        public int InsertGroup(Group group)
        {
            return GroupC.Insert(group).AsInt32;
        }
        #endregion

        #region IDatabase
        public ILiteCollection<BsonDocument> GetCollection(string collectionName)
        {
            return _liteDb.GetCollection<BsonDocument>(collectionName);
        }
        #endregion        
    }
}
