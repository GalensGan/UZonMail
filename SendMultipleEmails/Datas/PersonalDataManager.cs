using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    public class PersonalDataManager:ManagerBase
    {
        public PersonalData PersonalData { get; set; }
        public PersonalDataManager(Config config) : base(config) 
        {
            // 查看是否有数据，如果有，则加载，如果没有，则初始化
            if (File.Exists(config.UserDataDir + "\\" + config.userPersonalDataFileName))
            {
                PersonalData = ReadData<PersonalData,JObject>(config.UserDataDir + "\\" + config.userPersonalDataFileName);
            }
            else
            {
                PersonalData = new PersonalData()
                {
                    sendInterval = 5,
                };
            }

            // 检查是否为空
            if (PersonalData.variablesTable == null)
            {
                DataTable variables = new DataTable("variables");
                PersonalData.variablesTable = variables;
            }

            // 初始化发送人列表
            if (PersonalData.senders == null)
            {
                DataTable senders = new System.Data.DataTable("senders");
                senders.Columns.Add("Name", typeof(string));
                senders.Columns.Add("Email", typeof(string));
                senders.Columns.Add("Password", typeof(string));
                senders.Columns.Add("SMTP", typeof(string));
                senders.Columns.Add("IsAsSender", typeof(bool));
                PersonalData.senders = senders;
            }

            InitializeReceivers();
        }

        private void InitializeReceivers()
        {
            // 初始化收件人列表
            if (PersonalData.receivers == null)
            {
                DataTable receivers = new DataTable("receivers");
                receivers.Columns.Add("Name", typeof(string));
                receivers.Columns.Add("Email", typeof(string));
                receivers.Columns.Add("IsSendNow", typeof(bool));
                receivers.Columns.Add("SendStatus", typeof(bool));
                receivers.Columns.Add("SendDate", typeof(string));
                PersonalData.receivers = receivers;
            }
        }

        #region 发件人
        // 添加发件人
        public bool AddSender(Sender sender, bool saveRightNow)
        {
            if (PersonalData.senders.Select("Name='" + sender.Name + "'").Count() == 0)
            {
                DataRow row = PersonalData.senders.NewRow();
                row["Name"] = sender.Name;
                row["Email"] = sender.Email;
                row["Password"] = sender.Password;
                row["SMTP"] = sender.SMTP;
                row["IsAsSender"] = true;
                PersonalData.senders.Rows.Add(row);

                // 保存
                if (saveRightNow) Save();

                return true;
            }
            return false;
        }

        // 获取当前所有发件人
        public List<Sender> GetUsingSenders()
        {
            DataRow[] rows = PersonalData.senders.Select("IsAsSender = " + true+"");
            return rows.ToList().ConvertAll(row =>
            {
                Sender sender = new Sender()
                {
                    Name = row["Name"].ToString(),
                    Email = row["Email"].ToString(),
                    SMTP = row["SMTP"].ToString(),
                    Password = row["Password"].ToString()
                };
                return sender;
            });
        }
        #endregion

        #region 收件人
        // 添加收件人
        public bool AddReceiver(Person person, bool saveRightNow)
        {
            if (PersonalData.receivers.Select("Name='" + person.Name + "'").Count() == 0)
            {
                DataRow row = PersonalData.receivers.NewRow();
                row["Name"] = person.Name;
                row["Email"] = person.Email;
                row["IsSendNow"] = true;
                PersonalData.receivers.Rows.Add(row);

                // 保存
                if (saveRightNow) Save();

                return true;
            }
            return false;
        }


        // 获取所有收件人数量
        public int GetCurrentReceiverCount()
        {
            return GetCurrentReceiver().Length;
        }

        // 获取所有收件人
        public DataRow[] GetCurrentReceiver()
        {
            DataRow[] rows = PersonalData.receivers.Select("IsSendNow=" + true + "");
            return rows;
        }

        // 清空收件人
        public void ClearAllReceiver()
        {
            while (PersonalData.receivers.Rows.Count > 0)
            {
                PersonalData.receivers.Rows.RemoveAt(0);
            }
        }
        #endregion

        #region 收件人数据
        #endregion

        #region 公用方法

        /// <summary>
        /// 保存 PersonalData 数据
        /// </summary>
        public override bool Save()
        {
            return Save(Config.UserDataDir + "\\" + Config.userPersonalDataFileName, PersonalData);
        }
        #endregion
    }
}
