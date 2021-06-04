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

        public HistoryManager(DefaultConfig config) : base(config)
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
            if(!names.Contains(FieldKey.SenderName.ToString())) HistoryTable.Columns.Add(FieldKey.SenderName.ToString(), typeof(string));
            if (!names.Contains(FieldKey.SenderEmail.ToString())) HistoryTable.Columns.Add(FieldKey.SenderEmail.ToString(), typeof(string));
            if (!names.Contains(FieldKey.SenderSMTP.ToString())) HistoryTable.Columns.Add(FieldKey.SenderSMTP.ToString(), typeof(string));
            if (!names.Contains(FieldKey.ReceiverName.ToString())) HistoryTable.Columns.Add(FieldKey.ReceiverName.ToString(), typeof(string));
            if (!names.Contains(FieldKey.ReceiverEmail.ToString())) HistoryTable.Columns.Add(FieldKey.ReceiverEmail.ToString(), typeof(string));
            if (!names.Contains(FieldKey.SendDate.ToString())) HistoryTable.Columns.Add(FieldKey.SendDate.ToString(), typeof(DateTime));
            if (!names.Contains(FieldKey.IsSuccess.ToString())) HistoryTable.Columns.Add(FieldKey.IsSuccess.ToString(), typeof(bool));
            if (!names.Contains(FieldKey.Message.ToString())) HistoryTable.Columns.Add(FieldKey.Message.ToString(), typeof(string));
            // 一次发送是在一个组
            if (!names.Contains(FieldKey.GroupId.ToString())) HistoryTable.Columns.Add(FieldKey.GroupId.ToString(), typeof(int));
            if (!names.Contains(FieldKey.ResendEnabled.ToString())) HistoryTable.Columns.Add(FieldKey.ResendEnabled.ToString(), typeof(bool));
            if (!names.Contains(FieldKey.EmailBody.ToString())) HistoryTable.Columns.Add(FieldKey.EmailBody.ToString(), typeof(string));
            if (!names.Contains(FieldKey.EmailSubject.ToString())) HistoryTable.Columns.Add(FieldKey.EmailSubject.ToString(), typeof(string));

            // 获取最后的Index
            int count = HistoryTable.Rows.Count;
            if (count == 0) Index = 0;
            else
            {
                int index = int.Parse(HistoryTable.Rows[HistoryTable.Rows.Count - 1][FieldKey.GroupId.ToString()].ToString());
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
            row[FieldKey.SenderName.ToString()] = senderName;
            row[FieldKey.SenderEmail.ToString()] = senderEmail;
            row[FieldKey.SenderSMTP.ToString()] = senderSMTP;
            row[FieldKey.ReceiverName.ToString()] = receiverName;
            row[FieldKey.ReceiverEmail.ToString()] = receiverEmail;
            row[FieldKey.SendDate.ToString()] = sendDate;
            row[FieldKey.IsSuccess.ToString()] = isSuccess;
            row[FieldKey.Message.ToString()] = message;
            row[FieldKey.GroupId.ToString()] = groupId;
            row[FieldKey.ResendEnabled.ToString()] = true;
            row[FieldKey.EmailBody.ToString()] = emailBody;
            row[FieldKey.EmailSubject.ToString()] = emailSubject;

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
