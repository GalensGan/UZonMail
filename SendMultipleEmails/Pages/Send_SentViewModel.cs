using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SendMultipleEmails.Extension;
using Panuon.UI.Silver;
using log4net;
using System.Net.Mail;
using System.Net;

namespace SendMultipleEmails.Pages
{
    class Send_SentViewModel : SendScreenBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Send_SentViewModel));
        public Send_SentViewModel(Store store) : base(store) { }

        public BindingSource DataSource { get; set; }

        private List<string> _names;
        private string _filter;
        public override void Load()
        {
            // 从历史数据库中获取
            DataSource = new BindingSource
            {
                DataSource = Store.HistoryManager.HistoryTable,
            };

            // 获取所有的列头
            _names = Store.PersonalDataManager.GetTableNames(Store.HistoryManager.HistoryTable);
            // 设置过滤
            _filter = string.Format("{0} = {1}", History.GroupId.ToString(), Store.HistoryManager.Index);
            DataSource.Filter = _filter;
        }

        private List<Sender> _senders = null;
        /// <summary>
        /// 重新发送
        /// </summary>
        /// <param name="receiverName"></param>
        public void ReSend(DataRowView row)
        {
            // 添加重复发送线程
            if (_resendItems == null) _resendItems = new ConcurrentQueue<DataRowView>();
            if (_thread == null)
            {
                _signal = new AutoResetEvent(false);
                _thread = new Thread(new ThreadStart(() =>
                {
                    // 读取邮件，并发送
                    SendEmail();
                }));
                _thread.IsBackground = true;
                _thread.Start();
            }

            // 获取发件箱
            if (_senders == null || _senders.Count == 0)
            {
                _senders = Store.PersonalDataManager.GetUsingSenders().Distinct(s => s.Email).ToList();
                if (_senders.Count == 0)
                {
                    MessageBoxX.Show("请确认发件人列表，是否有可用发件人", "发件人错误");
                    return;
                }
            }

            // 添加进去
            // 设置 row 的 DeleteEnabled 属性
            row.Row[History.ResendEnabled.ToString()] = false;
            _resendItems.Enqueue(row);
            // 启动
            _signal.Set();
        }

        // https://www.cnblogs.com/chenwolong/p/AutoResetEvent.html
        private AutoResetEvent _signal;
        public void SendEmail()
        {
            int senderIndex = 0;
            while (true)
            {
                _signal.WaitOne();

                // 使用senders 轮流来发送
                // 从队列中获取数据
                if (_resendItems.TryDequeue(out DataRowView rowV))
                {
                    // 获取发送数据
                    Person receiver = new Person()
                    {
                        Name = rowV.Row[History.ReceiverName.ToString()].ToString(),
                        Email = rowV.Row[History.ReceiverEmail.ToString()].ToString(),
                    };

                    // 发送内容
                    string emailBody = rowV.Row[History.EmailBody.ToString()].ToString();
                    string emailSubject = rowV.Row[History.EmailSubject.ToString()].ToString();
                    int failure = 0;

                    // 发送数据
                    for (int i = senderIndex; i < (senderIndex+_senders.Count); i++)
                    {
                        try
                        {
                            Sender sender = _senders[i%_senders.Count];
                            _logger.Info(string.Format("[name:{0} - email:] 开始向 [name:{1} - email:{2}] 发送邮件",
                                sender.Name, sender.Email, receiver.Name, receiver.Email));

                            MailAddress fromAddr = new MailAddress(sender.Email);
                            MailAddress toAddr = new MailAddress(receiver.Email, receiver.Email);
                            MailMessage mailMsg = new MailMessage(fromAddr, toAddr)
                            {
                                Subject = emailSubject,
                                Body = emailBody,
                                IsBodyHtml = true,
                                BodyEncoding = Encoding.UTF8,
                            };

                            SmtpClient client = new SmtpClient
                            {
                                Host = sender.SMTP,
                                Port = Store.ConfigManager.AppConfig.smtpPort,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                Credentials = new NetworkCredential(sender.Email, sender.Password)
                            };                           
                            client.Send(mailMsg);

                            // 说明发送成功了，修改历史记录
                            rowV.Row[History.SenderName.ToString()] = sender.Name;
                            rowV.Row[History.SenderEmail.ToString()] = sender.Email;
                            rowV.Row[History.IsSuccess.ToString()] = true;
                            rowV.Row[History.Message.ToString()] = "已重发成功";
                            break;

                        }
                        catch
                        {
                            // 说明没有发送成功，继续用下一个账号发送
                            failure++;
                            if(failure<_senders.Count) continue;
                            else
                            {
                                // 如果所有邮箱都不成功，就标记失败，消息记录表明没有邮箱可以发件成功
                                rowV.Row[History.Message.ToString()] = "所有发件人都不能重发该邮件，请检查发件箱是否异常！";
                                rowV.Row[History.ResendEnabled.ToString()] = true;
                                break;
                            }
                        }
                    }                    
                }
                else
                {
                    _signal.Reset();
                }

                // 等待一定时间再发送
                Random rnd = new Random();
                double rndTime = (rnd.NextDouble() - 0.5) * Store.PersonalDataManager.PersonalData.sendIntervalRandom;
                if (rndTime < 0) rndTime = Math.Abs(rndTime);
                Thread.Sleep((int)((Store.PersonalDataManager.PersonalData.sendInterval + rndTime) * 1000 / _senders.Count));
            }
        }

        public void Back()
        {
            NextCommand(SendStatus.New);
        }

        public string FilterText { get; set; } = "";

        public void Filter()
        {
            string sql = string.Empty;
            for (int i = 0; i < _names.Count; i++)
            {
                if (i == 0)
                {
                    sql = string.Format("{0} LIKE '*{1}*'", _names[i], FilterText);
                }
                else sql += string.Format(" OR {0} LIKE '*{1}*'", _names[i], FilterText);
            }
            sql = string.Format("({0}) AND ({1})", _filter, sql);
            DataSource.Filter = sql;
        }

        #region 重新发送逻辑
        private ConcurrentQueue<DataRowView> _resendItems;
        private Thread _thread;
        #endregion
    }
}
