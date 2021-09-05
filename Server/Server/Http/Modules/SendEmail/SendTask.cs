using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Database;
using Server.Database.Models;
using Server.Http.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Http.Modules.SendEmail
{
    public class SendTask
    {
        public static SendTask Instance { get; private set; }

        // 新建发送任务
        public static bool CreateSendTask(string historyId, string userId, LiteDBManager liteDb, out string message)
        {
            // 判断原来任务的状态
            if (Instance != null && Instance.SendStatus >= SendStatus.Init)
            {
                message = "任务正在进行中";
                return false;
            }

            SendTask temp = new SendTask(historyId, userId, liteDb);

            Instance = temp;
            message = "success";
            return true;
        }

        private string _userId;
        private string _currentHistoryGroupId;
        private LiteDBManager _liteDb;

        public SendStatus SendStatus { get; private set; }

        private SendTask(string historyId, string userId, LiteDBManager liteDb)
        {
            _userId = userId;
            _currentHistoryGroupId = historyId;
            _liteDb = liteDb;
        }


        private SendingInfo _sendingInfo;
        public SendingInfo SendingInfo
        {
            get
            {
                if (_sendingInfo == null) _sendingInfo = new SendingInfo();
                return _sendingInfo;
            }
            set
            {
                // 判断是否已经完成
                if (value.index == value.total)
                {
                    this.SendStatus = SendStatus.SendFinish;
                    // 对于已经完成的，要更新数据的状态
                    var history = _liteDb.SingleById<HistoryGroup>(_currentHistoryGroupId);
                    if (history != null)
                    {
                        // 更新状态
                        history.sendStatus = SendStatus.SendFinish;
                        _liteDb.Update(history);

                        SendStatus = SendStatus.SendFinish;
                    }
                }

                _sendingInfo = value;
            }
        }

        private CancellationToken _cancleToken;

        /// <summary>
        /// 开始发送未发送成功的数据
        /// </summary>
        /// <param name="sendItemIds">传入需要重新发送的id</param>
        /// <returns></returns>
        public bool StartSending(string historyId)
        {
            // 判断是否结束
            if (SendStatus != SendStatus.SendFinish) return false;

            var allSendItems = _liteDb.Fetch<SendItem>(item => item.historyId == historyId);
            var sendItems = allSendItems.FindAll(item => !item.isSent);



            if (allSendItems.Count == sendItems.Count) SendStatus = SendStatus.Sending;
            else SendStatus = SendStatus.Resending;


            // 更改数据库中的状态
            var history = _liteDb.SingleById<HistoryGroup>(historyId);
            if (history == null) return false;

            history.sendStatus = SendStatus;
            _liteDb.Update(history);

            // 判断需要发送的数量
            if (allSendItems.Count < 1)
            {
                history.sendStatus = SendStatus.SendFinish;
                _liteDb.Update(history);

                // 获取重发完成的信息
                var sendingInfo = new SendingInfo()
                {
                    historyId = historyId,
                    index = 1,
                    total = 1,
                };
                SendingInfo = sendingInfo;

                return false;
            }

            // 开始发件
            SendItems(historyId, sendItems);

            return true;
        }

        private void SendItems(string historyId, List<SendItem> sendItemList)
        {
            var senders = _liteDb.Database.GetCollection<SendBox>().FindAll().ToList();

            // 添加到栈中
            Stack<SendItem> sendItems = new Stack<SendItem>();
            sendItemList.ForEach(item => sendItems.Push(item));

            // 初始化进度
            var sendingInfo0 = new SendingInfo()
            {
                historyId = historyId,
                index = 0,
                total = sendItemList.Count,
            };
            SendingInfo = sendingInfo0;

            _cancleToken = new CancellationToken();
            int sentIndex = 0;
            // 开始发送邮件，采用异步进行发送
            // 一个发件箱对应一个异步
            foreach (SendBox sb in senders)
            {
                Task.Run(() =>
                {
                    Setting setting = _liteDb.SingleOrDefault<Setting>(s => s.userId == _userId);
                    while (sendItems.Count > 0)
                    {
                        // 开始并行发送
                        //确定smtp服务器地址 实例化一个Smtp客户端
                        SmtpClient smtpclient = new SmtpClient();
                        smtpclient.Host = sb.smtp;
                        //smtpClient.Port = "";//qq邮箱可以不用端口

                        //确定发件地址与收件地址
                        MailAddress sendAddress = new MailAddress(sb.email);

                        // 获取发件箱
                        var sendItem = sendItems.Pop();
                        MailAddress receiveAddress = new MailAddress(sendItem.receiverEmail);

                        //构造一个Email的Message对象 内容信息
                        MailMessage mailMessage = new MailMessage(sendAddress, receiveAddress);
                        mailMessage.Subject = sendItem.subject;
                        mailMessage.SubjectEncoding = Encoding.UTF8;
                        mailMessage.Body = sendItem.html;
                        mailMessage.BodyEncoding = Encoding.UTF8;
                        mailMessage.IsBodyHtml = true;

                        //邮件发送方式  通过网络发送到smtp服务器
                        smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;

                        //如果服务器支持安全连接，则将安全连接设为true
                        smtpclient.EnableSsl = true;
                        try
                        {
                            //是否使用默认凭据，若为false，则使用自定义的证书，就是下面的networkCredential实例对象
                            smtpclient.UseDefaultCredentials = false;

                            //指定邮箱账号和密码,需要注意的是，这个密码是你在QQ邮箱设置里开启服务的时候给你的那个授权码
                            NetworkCredential networkCredential = new NetworkCredential(sb.email, sb.password);
                            smtpclient.Credentials = networkCredential;

                            //发送邮件
                            smtpclient.Send(mailMessage);
                            // 发送成功后，更新数据，更新到数据库
                            sendItem.senderEmail = sb.email;
                            sendItem.senderName = sb.userName;
                            sendItem.isSent = true;
                            sendItem.sendMessage = "邮件送达";
                            sendItem.sendDate = DateTime.Now;
                            _liteDb.Upsert(sendItem);

                            // 更新到当前进度中
                            var sendingInfo = new SendingInfo()
                            {
                                historyId = historyId,
                                index = ++sentIndex,
                                total = sendItemList.Count,
                                receiverEmail = sendItem.receiverEmail,
                                receiverName = sendItem.receiverName,
                                SenderEmail = sendItem.senderEmail,
                                SenderName = sendItem.senderName,
                            };
                            SendingInfo = sendingInfo;
                        }
                        catch (Exception ex)
                        {
                            // 超过最大尝试次数就退出
                            if (sendItem.tryCount > 5)
                            {
                                // 此时也要更新进度
                                var sendingInfo = new SendingInfo()
                                {
                                    historyId = historyId,
                                    index = ++sentIndex,
                                    total = sendItemList.Count,
                                    receiverEmail = sendItem.receiverEmail,
                                    receiverName = sendItem.receiverName,
                                    SenderEmail = sendItem.senderEmail,
                                    SenderName = sendItem.senderName,
                                };
                                SendingInfo = sendingInfo;
                            }
                            else if (setting.isAutoResend) // 重新发送时，才重新推入栈中
                            {
                                // 重新推入栈中
                                sendItems.Push(sendItem);
                            }

                            // 更新状态                      
                            sendItem.tryCount++;
                            sendItem.isSent = false;
                            if (ex.InnerException == null) sendItem.sendMessage = ex.Message;
                            else sendItem.sendMessage = ex.InnerException.Message;
                            _liteDb.Upsert(sendItem);
                        }
                        finally
                        {
                            // 每次发送完成，要等待一会儿再发送
                            double sleep = new Random().NextDouble() * 3 + 2;
                            if (setting != null)
                            {
                                sleep = setting.sendInterval_min + new Random().NextDouble() * (setting.sendInterval_max - setting.sendInterval_min);
                            }

                            Thread.Sleep((int)(sleep * 1000));
                        }
                    }
                }, _cancleToken);
            }
        }
    }
}
