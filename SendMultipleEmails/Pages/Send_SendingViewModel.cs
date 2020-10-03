using log4net;
using NPOI.OpenXmlFormats.Wordprocessing;
using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SendMultipleEmails.Extension;

namespace SendMultipleEmails.Pages
{
    class Send_SendingViewModel : SendScreenBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Send_SendingViewModel));
        public Send_SendingViewModel(Store store) : base(store) { }

        private List<Thread> _threads;
        private ManualResetEvent _event;
        private int _counter = 0;
        private int _success = 0;
        private int _failure = 0;

        private int _groupId = 0;
        private int _sendTotalCount;

        public int PMax { get; set; } = 100;
        public int PValue { get; set; } = 0;
        public override void Load()
        {
            CanResult = false;
            CanPause = true;
            CanCancle = true;
            _threads = new List<Thread>();
            _event = new ManualResetEvent(true);
            _counter = 0;
            _success = 0;
            _failure = 0;
            SendingLogs = new BindingList<string>();
            Store.HistoryManager.BeginAdd();
            _sendTotalCount = Store.PersonalDataManager.GetCurrentReceiverCount();
            // 设置进度条最大值
            PMax = _sendTotalCount;

            // 有多少个收件人，就开辟多少个线程进行发送
            List<Sender> senders = Store.PersonalDataManager.GetUsingSenders();
            // 对 senders 按邮箱去重
            senders = senders.Distinct(item => item.Email).ToList();

            if (senders.Count < 1)
            {
                MessageBoxX.Show("可发件人员数为0，退出发件", "发件人数量错误");
                NextCommand(Enums.SendStatus.New);
                return;
            }

            // 每个用户一个线程
            foreach (Sender sender in senders)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        _event.WaitOne();
                        if (Store.QueueReceivers.TryDequeue(out Tuple<Person, string> result))
                        {
                            // 读取之后开始发送
                            SendEmail(sender, result);
                        }
                        else // 读取不到数据后，就直接退出
                        {
                            return;
                        }
                        // 让发送时间跳跃，增加成功可能
                        Random rnd = new Random();
                        double rndTime = (rnd.NextDouble() - 0.5) * Store.PersonalDataManager.PersonalData.sendIntervalRandom;
                        if (rndTime < 0) rndTime = Math.Abs(rndTime);
                        int interval = (int)((Store.PersonalDataManager.PersonalData.sendInterval + rndTime) * 1000);

                        Execute.OnUIThreadSync(new Action(() =>
                        {
                            SendingLogs.Add(string.Format("[name:{0}-email:{1}] 将等待 {2}ms 之后开始准备发送下一封邮件",sender.Name,sender.Email,interval));
                        }));

                        Thread.Sleep(interval);
                    }
                }));
                thread.IsBackground = true;
                thread.Start();
                _threads.Add(thread);
            }
        }

        private void SendEmail(Sender sender, Tuple<Person, string> receiver)
        {
            try
            {
                Person rec = receiver.Item1;
                string startInfo = string.Format("[name:{0} - email:] 开始向 [name:{1} - email:{2}] 发送邮件",
                    sender.Name, sender.Email, receiver.Item1.Name, rec.Email);
                _logger.Info(startInfo);
                Execute.OnUIThreadSync(new Action(() =>
                {
                    SendingLogs.Add(startInfo);                   
                }));

                MailAddress fromAddr = new MailAddress(sender.Email);
                MailAddress toAddr = new MailAddress(rec.Email, rec.Email);
                MailMessage mailMsg = new MailMessage(fromAddr, toAddr)
                {
                    Subject = Store.MainTitle,
                    Body = receiver.Item2,
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
                //client.SendCompleted += Client_SendCompleted;
                //client.SendAsync(mailMsg, new Tuple<Sender, Person>(sender, rec));

                // 上面是异步形式，它可能会导致邮箱发送间隔时间不准确
                client.Send(mailMsg);

                // 发送成功
                _success++;
                string messageInfo = string.Format("[name:{0}-email:{1}] 发送成功！", receiver.Item1.Name, receiver.Item1.Email);
                _logger.Info(messageInfo);

                // 添加到历史记录中
                Store.HistoryManager.AddHistory(sender, receiver.Item1, Store.MainTitle,receiver.Item2, DateTime.Now, true, "发送成功");
                Execute.OnUIThreadSync(new Action(() =>
                {
                    SendingLogs.Add(messageInfo);
                    PValue = _failure + _success;
                    if (PValue == _sendTotalCount)
                    {
                        AllComplete();
                        return;
                    }
                }));
            }
            catch (SmtpException smtp)
            {
                _logger.Error(smtp.Message, smtp);                
                // 向数据库中写入失败信息
                Store.HistoryManager.AddHistory(sender, receiver.Item1, Store.MainTitle, receiver.Item2, DateTime.Now, false, smtp.Message);

                Execute.OnUIThreadSync(new Action(() =>
                {
                    SendingLogs.Add(string.Format("SMTP错误，[name:{0}email:{1}] 发送失败。原因：{2}", sender.Name, sender.Email, smtp.Message));
                    PValue = ++_failure + _success;
                    if (PValue == _sendTotalCount)
                    {
                        AllComplete();
                        return;
                    }
                }));
            }
            catch (Exception smtp)
            {
                _logger.Error(smtp.Message, smtp);
                // 向数据库中写入失败信息
                Store.HistoryManager.AddHistory(sender, receiver.Item1, Store.MainTitle, receiver.Item2, DateTime.Now, false, smtp.Message);

                Execute.OnUIThreadSync(new Action(() =>
                {
                    SendingLogs.Add(string.Format("SMTP错误，[name:{0}email:{1}] 发送失败。原因：{2}", sender.Name, sender.Email, smtp.Message));
                    PValue = ++_failure + _success;
                    if (PValue == _sendTotalCount)
                    {
                        AllComplete();
                        return;
                    }
                }));
            }
        }

        public BindingList<string> SendingLogs { get; set; }

        // 邮件发送成功后的回调
        //private void Client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    string messageInfo = string.Empty;
        //    if (e.UserState is Tuple<Sender, Person> persons)
        //    {
        //        if (e.Error == null)
        //        {
        //            _success++;
        //            messageInfo = string.Format("[name:{0}-email:{1}] 发送成功！", persons.Item2.Name, persons.Item2.Email);
        //            _logger.Info(messageInfo);

        //            // 添加到历史记录中
        //            Store.HistoryManager.AddHistory(persons.Item1, persons.Item2, DateTime.Now, true, "发送成功");
        //        }
        //        else
        //        {
        //            _failure++;
        //            messageInfo = string.Format("[name:{0}-email:{1}] 发送失败。Msg：{2}", persons.Item2.Name, persons.Item2.Email, e.Error.Message);
        //            _logger.Error(messageInfo, e.Error);
        //            // 添加到历史记录中
        //            Store.HistoryManager.AddHistory(persons.Item1, persons.Item2, DateTime.Now, false, e.Error.Message);
        //        }
        //    }

        //    Execute.OnUIThreadSync(new Action(() =>
        //    {
        //        SendingLogs.Add(messageInfo);
        //        PValue = _failure + _success;
        //        if (PValue == _sendTotalCount)
        //        {
        //            AllComplete();
        //            return;
        //        }
        //    }));
        //}

        // 完成

        private void AllComplete()
        {
            // 设置进度条
            PValue = PMax;
            // 保存历史记录
            Store.HistoryManager.Save();

            string info = string.Format("本次共发送 [{0}] 条信息，成功 [{1}] 条，失败 [{2}] 条。", _sendTotalCount, _success, _failure);
            MessageBoxX.Show(info, "发送结束");

            NextCommand(Enums.SendStatus.Sent);
        }

        public string Log { get; set; }

        public bool CanResult { get; set; } = true;

        public void Result()
        {
            Store.HistoryManager.Save();
            NextCommand(Enums.SendStatus.Sent);
        }

        public bool CanPause { get; set; } = true;
        public string PauseText { get; set; } = "暂停";
        public void Pause()
        {
            if (PauseText == "暂停") PauseText = "继续";
            else PauseText = "暂停";

            // 根据情况，来进行操作
            if (PauseText == "暂停")
            {
                _event.Reset();
            }
            else
            {
                _event.Set();
            }
        }

        public bool CanCancle { get; set; } = true;
        public Visibility ShowResult { get; set; } = Visibility.Collapsed;
        public Visibility ShowPause { get; set; } = Visibility.Visible;
        public void Cancle()
        {
            _threads.ForEach(t => t.Abort());

            CanResult = true;

            ShowResult = Visibility.Visible;
            ShowPause = Visibility.Collapsed;
        }
    }
}
