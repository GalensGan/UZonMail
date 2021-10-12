using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Database;
using Server.Database.Models;
using Server.Http.Definitions;
using Server.Protocol;
using Server.Websocket.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Group = Server.Database.Models.Group;

namespace Server.Http.Modules.SendEmail
{
    /// <summary>
    /// 发件中心
    /// 每个用户每次只能有一个任务在运行
    /// 对于定时任务，只能注册一个事件来回调
    /// </summary>
    public class SendTask
    {
        // 新建发送任务
        public static bool CreateSendTask(string historyId, string userId, LiteDBManager liteDb, out string message)
        {
            // 判断原来任务的状态
            if (InstanceCenter.SendTasks[userId] != null && !InstanceCenter.SendTasks[userId].SendStatus.HasFlag(SendStatus.SendFinish))
            {
                message = "任务正在进行中";
                return false;
            }

            SendTask temp = new SendTask(historyId, userId, liteDb);

            InstanceCenter.SendTasks.Upsert(userId, temp);
            message = "success";
            return true;
        }

        private string _userId;
        private string _currentHistoryGroupId;
        private LiteDBManager _liteDb;


        /// <summary>
        /// 发送状态
        /// </summary>
        public SendStatus SendStatus { get; private set; } = SendStatus.SendFinish;


        private SendTask(string historyId, string userId, LiteDBManager liteDb)
        {
            _userId = userId;
            _currentHistoryGroupId = historyId;
            _liteDb = liteDb;
        }


        private SendingProgressInfo _sendingInfo;
        public SendingProgressInfo SendingProgressInfo
        {
            get
            {
                if (_sendingInfo == null) _sendingInfo = new SendingProgressInfo();
                return _sendingInfo;
            }
            set
            {
                _sendingInfo = value;

                // 进度websocket版本
                // 将进度发给websocket
                SendCallback.Insance.Send(_userId, new Protocol.Response()
                {
                    result = value,
                    eventName = "sendingInfo",
                    command = "updatePorgress",
                });
            }
        }


        /// <summary>
        /// 开始发送未发送成功的数据
        /// </summary>
        /// <param name="sendItemIds">传入需要重新发送的id</param>
        /// <returns></returns>
        public bool StartSending()
        {
            // 判断是否结束
            if (!SendStatus.HasFlag(SendStatus.SendFinish)) return false;

            var allSendItems = _liteDb.Fetch<SendItem>(item => item.historyId == _currentHistoryGroupId);
            var sendItems = allSendItems.FindAll(item => !item.isSent);
            // 判断数量
            if (sendItems.Count < 1)
            {
                // 发送完成的进度条
                SendingProgressInfo = new SendingProgressInfo()
                {
                    total = 1,
                    index = 1,
                };
                return true;
            }
            else
            {
                // 更改进度
                SendingProgressInfo = new SendingProgressInfo()
                {
                    total = sendItems.Count,
                    index = 0,
                };
            }

            // 判断是发送还是重发
            if (allSendItems.Count == sendItems.Count) SendStatus = SendStatus.Sending;
            else SendStatus = SendStatus.Resending;

            // 更改数据库中的状态
            var history = _liteDb.SingleById<HistoryGroup>(_currentHistoryGroupId);
            if (history == null) return false;

            history.sendStatus = SendStatus;
            _liteDb.Update(history);


            // 判断需要发送的数量
            if (allSendItems.Count < 1)
            {
                history.sendStatus = SendStatus.SendFinish;
                _liteDb.Update(history);

                // 获取重发完成的信息
                var sendingInfo = new SendingProgressInfo()
                {
                    historyId = _currentHistoryGroupId,
                    index = 1,
                    total = 1,
                };
                SendingProgressInfo = sendingInfo;

                return false;
            }

            // 处理每条邮件
            PreHandleSendItems(sendItems);

            // 开始发件
            SendItems(sendItems);

            return true;
        }

        /// <summary>
        /// 对发件进行预处理
        /// </summary>
        /// <param name="sendItems"></param>
        private void PreHandleSendItems(List<SendItem> sendItems)
        {
            if (sendItems.Count < 1) return;

            // 获取设置
            Setting setting = _liteDb.SingleOrDefault<Setting>(s => s.userId == _userId);

            // 设置发送的内容           
            if (setting.sendWithImageAndHtml) SendStatus |= SendStatus.AsImage;
            else SendStatus |= SendStatus.AsHtml;

            // 奇偶混发
            for (int index = 0; index < sendItems.Count; index++)
            {
                var sendItem = sendItems[index];
                // 偶数发图片
                // 如果被设置了发送类型，就按设置的发送类型进行发送
                if (index % 2 == 0 && setting.sendWithImageAndHtml && sendItem.sendItemType == SendItemType.none)
                {
                    sendItem.sendItemType = SendItemType.dataUrl;
                }
            }

            // 初始化进度
            var sendingInfo0 = new SendingProgressInfo()
            {
                historyId = _currentHistoryGroupId,
                index = 0,
                total = sendItems.Count,
            };

            SendingProgressInfo = sendingInfo0;
        }


        /// <summary>
        /// 邮件发送结束
        /// </summary>
        public event Action SendCompleted;

        private void SendItems(List<SendItem> sendItemList)
        {
            if (sendItemList.Count < 0) return;

            var historyGroup = _liteDb.Database.GetCollection<HistoryGroup>().FindById(_currentHistoryGroupId);

            // 添加到栈中
            Stack<SendItem> sendItemStack = new Stack<SendItem>();

            // 栈是先进后出，所以要倒转一下
            sendItemList.Reverse();
            sendItemList.ForEach(item => sendItemStack.Push(item));

            // 获取发件人
            List<SendBox> senders = _liteDb.Fetch<SendBox>(sb => historyGroup.senderIds.Contains(sb._id));

            // 开始发送邮件，采用异步进行发送
            // 一个发件箱对应一个异步
            List<SendThread> sendThreads = senders.ConvertAll(sender =>
            {
                var sendThread = new SendThread(_userId, sender, _liteDb);
                sendThread.SendCompleted += SendThread_SendCompleted;
                return sendThread;
            });

            // 开始运行
            Task.WhenAll(sendThreads.ConvertAll(st => st.Run(sendItemStack))).ContinueWith((task) =>
            {
                // 执行回调
                // 发送关闭命令
                SendStatus = SendStatus.SendFinish;

                // 对于已经完成的，要更新数据的状态
                var history = _liteDb.SingleById<HistoryGroup>(_currentHistoryGroupId);
                if (history != null)
                {
                    // 更新状态
                    history.sendStatus = SendStatus.SendFinish;
                    _liteDb.Update(history);
                }

                // 发送完成数据
                SendingProgressInfo = new SendingProgressInfo()
                {
                    historyId = _currentHistoryGroupId,
                    index = sendItemList.Count,
                    total = sendItemList.Count,
                };

                SendCompleted?.Invoke();
            });
        }

        private void SendThread_SendCompleted(SendResult obj)
        {
            // 单个发送结束后的事件
            // 发送进度条
            SendingProgressInfo = new SendingProgressInfo()
            {
                total = SendingProgressInfo.total,
                index = SendingProgressInfo.index + 1,
                historyId = _currentHistoryGroupId,
                receiverEmail = obj.SendItem.receiverEmail,
                receiverName = obj.SendItem.receiverName,
                SenderEmail = obj.SendBox.email,
                SenderName = obj.SendBox.userName
            };
        }
    }
}
