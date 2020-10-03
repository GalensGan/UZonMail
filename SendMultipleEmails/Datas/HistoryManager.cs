using mshtml;
using Newtonsoft.Json.Linq;
using SendMultipleEmails.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    // 历史记录
    public class HistoryManager : ManagerBase
    {
        // 历史管理
        public DataTable HistoryTable { get; private set; }
        public int Index { get; private set; }

        public HistoryManager(Config config) : base(config)
        {
            // 从数据文件读取
            if (File.Exists(config.UserHistoryPath))
            {
                HistoryTable = ReadData<DataTable, JArray>(config.UserHistoryPath);
            }

            if (HistoryTable == null)
            {
                // 生成表
                HistoryTable = new DataTable("history");               
            }

            // 检查表中的各个字段
            List<string> names = base.GetTableNames(this.HistoryTable);
            if(!names.Contains(History.SenderName.ToString())) HistoryTable.Columns.Add(History.SenderName.ToString(), typeof(string));
            if (!names.Contains(History.SenderEmail.ToString())) HistoryTable.Columns.Add(History.SenderEmail.ToString(), typeof(string));
            if (!names.Contains(History.SenderSMTP.ToString())) HistoryTable.Columns.Add(History.SenderSMTP.ToString(), typeof(string));
            if (!names.Contains(History.ReceiverName.ToString())) HistoryTable.Columns.Add(History.ReceiverName.ToString(), typeof(string));
            if (!names.Contains(History.ReceiverEmail.ToString())) HistoryTable.Columns.Add(History.ReceiverEmail.ToString(), typeof(string));
            if (!names.Contains(History.SendDate.ToString())) HistoryTable.Columns.Add(History.SendDate.ToString(), typeof(DateTime));
            if (!names.Contains(History.IsSuccess.ToString())) HistoryTable.Columns.Add(History.IsSuccess.ToString(), typeof(bool));
            if (!names.Contains(History.Message.ToString())) HistoryTable.Columns.Add(History.Message.ToString(), typeof(string));
            // 一次发送是在一个组
            if (!names.Contains(History.GroupId.ToString())) HistoryTable.Columns.Add(History.GroupId.ToString(), typeof(int));
            if (!names.Contains(History.ResendEnabled.ToString())) HistoryTable.Columns.Add(History.ResendEnabled.ToString(), typeof(bool));
            if (!names.Contains(History.EmailBody.ToString())) HistoryTable.Columns.Add(History.EmailBody.ToString(), typeof(string));
            if (!names.Contains(History.EmailSubject.ToString())) HistoryTable.Columns.Add(History.EmailSubject.ToString(), typeof(string));

            // 获取最后的Index
            int count = HistoryTable.Rows.Count;
            if (count == 0) Index = 0;
            else
            {
                int index = int.Parse(HistoryTable.Rows[HistoryTable.Rows.Count - 1][History.GroupId.ToString()].ToString());
                Index = index;
            }
        }

        /// <summary>
        /// 在调用之前，先调用BeginAdd，一轮添加结束后，要调用 BeginEnd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="sendDate"></param>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool AddHistory(Sender sender, Person receiver,string emailSubject, string emailBody, DateTime sendDate, bool isSuccess, string message, int groupId=-1)
        {
            return this.AddHistory(sender.Name, sender.Email, sender.SMTP, receiver.Name, receiver.Email, emailSubject, emailBody,sendDate, isSuccess, message, groupId);
        }

        /// <summary>
        /// 在调用之前，先调用BeginAdd，一轮添加结束后，要调用 BeginEnd
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderEmail"></param>
        /// <param name="senderSMTP"></param>
        /// <param name="receiverName"></param>
        /// <param name="receiverEmail"></param>
        /// <param name="sendDate"></param>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool AddHistory(string senderName, string senderEmail, string senderSMTP, string receiverName, string receiverEmail, string emailSubject, string emailBody, DateTime sendDate, bool isSuccess, string message, int groupId)
        {
            if (!_isBegin) return false;

            if (groupId < 0) groupId = Index;

            DataRow row = HistoryTable.NewRow();
            row[History.SenderName.ToString()] = senderName;
            row[History.SenderEmail.ToString()] = senderEmail;
            row[History.SenderSMTP.ToString()] = senderSMTP;
            row[History.ReceiverName.ToString()] = receiverName;
            row[History.ReceiverEmail.ToString()] = receiverEmail;
            row[History.SendDate.ToString()] = sendDate;
            row[History.IsSuccess.ToString()] = isSuccess;
            row[History.Message.ToString()] = message;
            row[History.GroupId.ToString()] = groupId;
            row[History.ResendEnabled.ToString()] = true;
            row[History.EmailBody.ToString()] = emailBody;
            row[History.EmailSubject.ToString()] = emailSubject;

            HistoryTable.Rows.Add(row);
            return true;
        }

        private bool _isBegin = false;
        public int BeginAdd()
        {
            _isBegin = true;
            return ++Index;
        }
        public int EndAdd()
        {
            _isBegin = false;
            return Index;
        }

        public DataRow[] Select(string sql)
        {
            return HistoryTable.Select(sql);
        }

        public override bool Save()
        {
            return base.Save(Config.UserHistoryPath, this.HistoryTable);
        }
    }
}
