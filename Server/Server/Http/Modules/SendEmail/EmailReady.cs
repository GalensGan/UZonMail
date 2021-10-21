using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Database;
using Server.Database.Models;
using Server.Http.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Modules.SendEmail
{
    class EmailReady : EmailPreview
    {
        public static bool CreateEmailReady(string userId,JToken data, LiteDBManager liteDb, out string message)
        {
            // 判断是否有发送任务正在进行
            if (InstanceCenter.SendTasks[userId] != null && !InstanceCenter.SendTasks[userId].SendStatus.HasFlag(SendStatus.SendFinish))
            {
                message = "有邮件正在发送中";
                return false;
            }

            EmailReady temp = new EmailReady(userId,data, liteDb);

            InstanceCenter.EmailReady.Upsert(userId, temp);

            message = "success";
            return true;
        }

        private string _userId;
        public EmailReady(string userId,JToken data, LiteDBManager liteDb) : base(data, liteDb)
        {
            _userId = userId;
        }

        private GenerateInfo _info;

        public override GenerateInfo Generate()
        {
            _info = new GenerateInfo();

            // 覆盖生成逻辑,不能用异步，因为要计算实际发件量
            GenerateSendItems();

            return _info;
        }

        /// <summary>
        /// 生成之后的操作
        /// </summary>
        /// <param name="sendItems"></param>
        /// <param name="receiveBoxes"></param>
        protected override void PreviewItemCreated(List<SendItem> sendItems, List<ReceiveBox> receiveBoxes)
        {
            // 生成发送的组
            // 获取发件箱

            // 判断数据中是否有发件人
            var senderIds = new List<string>();
            List<SendBox> senders = null;
            if (senderIds.Count > 0)
            {
                // 使用选择发件人发件
                senders = LiteDb.Fetch<SendBox>(sd => senderIds.Contains(sd._id));
            }
            else
            {
                senders = LiteDb.Database.GetCollection<SendBox>().FindAll().ToList();
            }

            // 添加历史
            HistoryGroup historyGroup = new HistoryGroup()
            {
                userId = _userId,
                createDate = DateTime.Now,
                subject = Subject,
                data = JsonConvert.SerializeObject(Data),
                receiverIds = receiveBoxes.ConvertAll(rec => rec._id),
                templateId = Template._id,
                templateName = Template.name,
                senderIds = senders.ConvertAll(s => s._id),
                sendStatus = SendStatus.Sending,
            };

            LiteDb.Database.GetCollection<HistoryGroup>().Insert(historyGroup);

            // 反回发件信息
            _info.historyId = historyGroup._id;

            // 如果选择发件人，默认从数据中读取发件人，所以选择的发件人数量为0
            if (Receivers == null || Receivers.Count < 1) _info.selectedReceiverCount = 0;
            else _info.selectedReceiverCount = receiveBoxes.Count;

            _info.dataReceiverCount = Data.Count;
            _info.acctualReceiverCount = sendItems.Count;
            _info.ok = true;
            _info.senderCount = senders.Count;

            // 将所有的待发信息添加到数据库
            sendItems.ForEach(item => item.historyId = historyGroup._id);
            LiteDb.Database.GetCollection<SendItem>().InsertBulk(sendItems);
        }
    }
}
