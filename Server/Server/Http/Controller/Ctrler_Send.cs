using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json.Linq;
using Server.Database;
using Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    /// <summary>
    /// 本类主要用于发送，一次只能一个发送任务
    /// </summary>
    public class Ctrler_Send : BaseController
    {
        // 获取发送状态，用于是否恢复界面的发送情况
        [Route(HttpVerbs.Get, "/send/status")]
        public void GetSendStatus()
        {
            SendStatus sendStatus = SendStatus.Init;
            if (SendTask.Instance != null)
            {
                sendStatus = SendTask.Instance.SendStatus;
            }

            ResponseSuccess(sendStatus);
        }


        // 新建发送任务
        [Route(HttpVerbs.Post, "/send/task")]
        public void CreateTask()
        {
            // 生成
            string subject = Body.Value<string>("subject");
            JArray receivers = Body.Value<JArray>("receivers");
            JArray data = Body.Value<JArray>("data");
            int templateId = Body.Value<int>("templateId");

            bool createResult = SendTask.CreateSendTask(Token.UserId, subject, receivers, data, templateId, LiteDb, out string message);
            if (createResult) ResponseSuccess(createResult);
            else ResponseError(message);
        }

        // 获取预览内容
        [Route(HttpVerbs.Get, "/send/preview/{key}")]
        public void GetSendStatus(string key)
        {
            SendItem item = SendTask.Instance.GetPreviewHtml(key);
            if (item == null)
            {
                ResponseError("没有可预览项，请检查收件箱是否为空");
                return;
            }

            ResponseSuccess(item);
        }

        // 开始发送邮件
        [Route(HttpVerbs.Post, "/send")]
        public void StartSending(string key)
        {
            var id = SendTask.Instance.StartSending();
            ResponseSuccess(id);
        }

        // 获取发件状态
        [Route(HttpVerbs.Get, "/send/info")]
        public void GetSendingInfo()
        {
            ResponseSuccess(SendTask.Instance.SendingInfo);
        }
    }

    class SendTask
    {
        public static SendTask Instance { get; private set; }

        public static bool CreateSendTask(string userId, string subject, JArray receivers, JArray data, int templateId, LiteDBManager liteDb, out string message)
        {
            // 判断原来任务的状态
            if (Instance != null && Instance.SendStatus >= SendStatus.Sending && Instance.SendStatus < SendStatus.SendFinish)
            {
                message = "任务正在进行中";
                return false;
            }

            SendTask temp = new SendTask(userId, subject, receivers, data, templateId, liteDb);
            // 判断是否符合数据要求
            if (!temp.Validate(out string validateMsg))
            {
                message = validateMsg;
                return false;
            }

            Instance = temp;
            message = "success";
            return true;
        }

        private string _userId;
        private string _subject;
        private JArray _receivers;
        private JArray _data;
        private Template _template;
        private LiteDBManager _liteDb;
        private List<SendItem> _sendItems;

        public SendStatus SendStatus { get; private set; }

        private SendTask(string userId, string subject, JArray receivers, JArray data, int templateId, LiteDBManager liteDb)
        {
            _userId = userId;
            _subject = subject;
            _receivers = receivers;
            _data = data;
            _liteDb = liteDb;

            // 获取模板
            _template = _liteDb.SingleOrDefault<Template>(t => t._id == templateId);

            // 生成发件模板，用异步，否则会阻塞该线程
            new Task(() =>
            {
                // 生成每一项
                _sendItems = new List<SendItem>();
                List<ReceiveBox> receiveBoxes = new List<ReceiveBox>();
                // 获取当前收件人或组下的所有人
                foreach (JToken jt in _receivers)
                {
                    // 判断 type
                    string type = jt.Value<string>("type");
                    int id = jt.Value<int>("_id");
                    if (type == "group")
                    {
                        // 找到group下所有的用户
                        List<ReceiveBox> boxes = liteDb.Fetch<ReceiveBox>(r => r.groupId == id);
                        receiveBoxes.AddRange(boxes);
                    }
                    else
                    {
                        // 选择了单个用户
                        var box = liteDb.SingleOrDefault<ReceiveBox>(r => r._id == id);
                        if (box != null) receiveBoxes.Add(box);
                    }
                }

                // 开始添加                
                foreach (var re in receiveBoxes)
                {
                    // 判断有没有数据
                    var itemData = _data.FirstOrDefault(jt => jt.Value<string>("userName") == re.userName);
                    if (itemData == null) continue;

                    var item = new SendItem()
                    {
                        subject = _subject,
                        receiverName = re.userName,
                        receiverEmail = re.email,
                    };

                    // 获取数据
                    List<string> keys = (itemData as JObject).Properties().ToList().ConvertAll(p => p.Name);
                    // 对所有数据进行替换
                    string sendHtml = _template.html;
                    foreach (string key in keys)
                    {
                        var regex = new Regex("{{\\s*" + key + "\\s*}}");
                        sendHtml = regex.Replace(sendHtml, itemData[key].Value<string>());
                    }

                    item.html = sendHtml;

                    // 添加到保存的集合中
                    _sendItems.Add(item);
                };

                // 添加序号
                for (int i = 0; i < _sendItems.Count; i++)
                {
                    _sendItems[i].index = i;
                    _sendItems[i].total = _sendItems.Count;
                }
            }).Start();
        }

        private bool Validate(out string message)
        {
            message = "success";
            return true;
        }

        private int _index;

        // 获取预览内容
        public SendItem GetPreviewHtml(string directive)
        {
            if (_sendItems.Count < 1) return null;

            switch (directive)
            {
                case "first":
                    _index = 0;
                    return _sendItems.FirstOrDefault();
                case "next":
                    _index++;
                    return _sendItems[CycleInt(_index, _sendItems.Count)];
                case "previous":
                    _index--;
                    return _sendItems[CycleInt(_index, _sendItems.Count)];
                default:
                    // 通过名字来搜索
                    return null;
            }
        }

        private int CycleInt(int index, int total)
        {
            int result = index % total;
            if (result >= 0) return result;
            return total + result;
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
                value.total = _sendItems.Count;

                if (value.index == value.total)
                {
                    this.SendStatus = SendStatus.SendFinish;
                    Instance = null;
                }

                _sendingInfo = value;
            }
        }

        private CancellationToken _cancleToken;
        public int StartSending()
        {
            // 获取发件箱
            var senders = _liteDb.Database.GetCollection<SendBox>().FindAll().ToList();

            // 添加历史
            HistoryGroup historyGroup = new HistoryGroup()
            {
                userId = _userId,
                createDate = DateTime.Now,
                subject = _subject,
                data = _data,
                ReceiverIds = _receivers.ToObject<LinkedList<ReceiveBox>>().ToList().ConvertAll(rec => rec._id),
                templateId = _template._id,
                senderIds = senders.ConvertAll(s => s._id),
            };

            // 将所有的待发信息添加到数据库，然后读取出来批量发送
            _sendItems.ForEach(item => item.historyId = historyGroup._id);
            _liteDb.Database.GetCollection<SendItem>().InsertBulk(_sendItems);

            // 添加到栈中
            Stack<SendItem> sendItems = new Stack<SendItem>();
            _sendItems.ForEach(item => sendItems.Push(item));

            _cancleToken = new CancellationToken();
            int successCount = 0;
            // 开始发送邮件，采用异步进行发送
            // 一个发件箱对应一个异步
            foreach (SendBox sb in senders)
            {
                Task.Run(() =>
                {
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
                            sendItem.sendMessage = "success";
                            _liteDb.Upsert(sendItem);

                            // 更新到当前进度中
                            var sendingInfo = new SendingInfo()
                            {
                                index = successCount++,
                                receiverEmail = sendItem.receiverEmail,
                                receiverName = sendItem.receiverName,
                                SenderEmail = sendItem.senderEmail,
                                SenderName = sendItem.senderName,
                            };
                            SendingInfo = sendingInfo;
                        }
                        catch (SmtpException ex)
                        {
                            // 重新推入栈中
                            sendItem.tryCount++;
                            sendItem.isSent = false;
                            sendItem.sendMessage = ex.Message;
                            _liteDb.Upsert(sendItem);

                            // 超过最大尝试次数就退出
                            if (sendItem.tryCount > 5)
                            {
                                return;
                            }
                            else
                            {
                                sendItems.Push(sendItem);
                            }
                        }
                    }


                }, _cancleToken).Start();
            }

            return historyGroup._id;
        }
    }

    enum SendStatus
    {
        Init,
        Sending,
        Pause,
        SendFinish
    }

    public class SendingInfo
    {
        public int index { get; set; } = 0;
        public int total { get; set; } = 1;
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string receiverName { get; set; }
        public string receiverEmail { get; set; }
    }
}
