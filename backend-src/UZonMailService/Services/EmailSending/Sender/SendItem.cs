using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Timers;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.EmailSending;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.WaitList;
using UZonMailService.SignalRHubs;
using UZonMailService.SignalRHubs.Extensions;
using UZonMailService.SignalRHubs.SendEmail;
using Timer = System.Timers.Timer;
using UZonMailService.Utils.Database;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 发件项
    /// </summary>
    public class SendItem
    {
        public SqlContext SqlContext { get; set; }
        public IHubContext<UzonMailHub, IUzonMailClient> Hub { get; set; }
        public ILogger Logger { get; set; }

        public SendingItem SendingItem { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sendingItem"></param>
        public SendItem(SendingItem sendingItem)
        {
            SendingItem = sendingItem;

            BCC = sendingItem.BCC;
            CC = sendingItem.CC;
            Inboxes = sendingItem.Inboxes;
            IsSendingBatch = sendingItem.IsSendingBatch;

            if (sendingItem.Attachments != null)
                FileUsageIds = sendingItem.Attachments.Select(x => x.Id).ToList();
        }

        /// <summary>
        /// 发送类型
        /// </summary>
        public SendItemType SendItemType { get; set; }

        /// <summary>
        /// 发件箱
        /// </summary>
        public OutboxEmailAddress Outbox { get; set; }

        /// <summary>
        /// 收件箱
        /// </summary>
        public List<EmailAddress> Inboxes { get; set; }

        /// <summary>
        /// 抄送人
        /// </summary>
        public List<EmailAddress>? CC { get; set; }

        /// <summary>
        /// 密送人
        /// </summary>
        public List<EmailAddress>? BCC { get; set; }

        /// <summary>
        /// 回复的邮箱
        /// </summary>
        public List<string> ReplyToEmails { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { private get; set; }

        /// <summary>
        /// HTML 内容
        /// </summary>
        public string HtmlBody { private get; set; }

        /// <summary>
        /// 正文变量数据
        /// </summary>
        public SendingItemExcelData? BodyData { get; set; }

        /// <summary>
        /// 附件 FileUsageId 列表
        /// </summary>
        public List<long> FileUsageIds { get; set; }

        /// <summary>
        /// 批量发送
        /// </summary>
        public bool IsSendingBatch { get; set; }

        /// <summary>
        /// 代理信息
        /// </summary>
        public ProxyInfo? ProxyInfo { get; set; }

        /// <summary>
        /// 验证数据是否满足要求
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(Outbox.Email) || Inboxes == null || Inboxes.Count == 0
                || string.IsNullOrEmpty(Outbox.SmtpHost) || Outbox.SmtpPort == 0
                || string.IsNullOrEmpty(HtmlBody))
            {
                return false;
            }
            return true;
        }

        private List<Tuple<string, string>> _attachments;
        /// <summary>
        /// 获取附件
        /// </summary>
        /// <returns></returns>
        public async Task<List<Tuple<string, string>>> GetAttachments()
        {
            if (_attachments != null)
            {
                return _attachments;
            }

            // 查找文件
            var attachments = await SqlContext.FileUsages.Where(f => FileUsageIds.Contains(f.Id))
               .Include(x => x.FileObject)
               .ThenInclude(x => x.FileBucket)
               .Select(x => new { fullPath = $"{x.FileObject.FileBucket.RootDir}/{x.FileObject.Path}", fileName = x.DisplayName ?? x.FileName })
               .ToListAsync();
            _attachments = [];
            foreach (var item in attachments)
            {
                _attachments.Add(new Tuple<string, string>(item.fullPath, item.fileName));
            }

            return _attachments;
        }

        private string _body;
        /// <summary>
        /// 获取经过变量替换后的正文
        /// 在发送时调用
        /// </summary>
        /// <returns></returns>
        public string GetBody()
        {
            if (!string.IsNullOrEmpty(_body)) return _body;
            // 替换正文变量
            _body = ComputedVariables(HtmlBody);
            return _body;
        }

        private string ComputedVariables(string originText)
        {
            if (string.IsNullOrEmpty(originText)) return originText;
            // 替换正文变量
            if (BodyData == null) return originText;

            foreach (var item in BodyData)
            {
                if (item.Value == null) continue;
                // 使用正则进行替换
                var regex = new Regex(@"\{\{\s*" + item.Key + @"\s*\}\}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                originText = regex.Replace(originText, item.Value.ToString());
            }
            return originText;
        }

        private string _subject;
        /// <summary>
        /// 获取主题
        /// </summary>
        /// <returns></returns>
        public string GetSubject()
        {
            if (!string.IsNullOrEmpty(_subject)) return _subject;

            // 主题中可能有变量
            _subject = ComputedVariables(Subject);
            return _subject;
        }

        #region 重发逻辑，该部分仅在主服务器上使用，后期考虑抽象出来
        // 重试次数
        private int _triedCount = 0;
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int RetryMax = 0;

        /// <summary>
        /// 保存发送状态
        /// 返回状态指示是否需要重试
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<SentStatus> UpdateSendingStatus(SendCompleteResult sendCompleteResult)
        {
            // 判断是否需要重试
            if (!sendCompleteResult.Ok && _triedCount < RetryMax)
            {
                _triedCount++;
                return SentStatus.Retry;
            }
            // 保存到数据库
            SendingItem updatedItem = await SaveSendItemInfos(sendCompleteResult);

            // 通知发送结果
            var client = Hub.GetUserClient(SendingItem.UserId);
            if (client != null)
            {
                await client.SendingItemStatusChanged(new SendingItemStatusChangedArg(updatedItem));
            }

            // 调用回调,通知上层处理结果
            await _sendGroupTask.EmailItemSendCompleted(sendCompleteResult);
            return sendCompleteResult.Ok ? SentStatus.OK : SentStatus.Failed;
        }

        /// <summary>
        /// 保存 SendItem 状态
        /// </summary>
        /// <returns></returns>
        private async Task<SendingItem> SaveSendItemInfos(SendCompleteResult sendCompleteResult)
        {
            var db = sendCompleteResult.SqlContext;
            var success = sendCompleteResult.Ok;
            var message = sendCompleteResult.Message;

            // 更新 sendingItems 状态            
            var data = await db.SendingItems.FirstOrDefaultAsync(x => x.Id == SendingItem.Id);
            // 更新数据
            data.FromEmail = Outbox.Email;
            data.Subject = GetSubject();
            data.Content = GetBody();
            // 保存发送状态
            data.Status = success ? SendingItemStatus.Success : SendingItemStatus.Failed;
            data.SendResult = message;
            data.TriedCount = _triedCount;
            data.SendDate = DateTime.Now;
            // 解析邮件 id
            data.ReceiptId = new ResultParser(message).GetReceiptId();

            // 更新 sendingItemInbox 状态
            await db.SendingItemInboxes.UpdateAsync(x => x.SendingItemId == SendingItem.Id,
                x => x.SetProperty(y => y.FromEmail, Outbox.Email)
                    .SetProperty(y => y.SendDate, DateTime.Now)
                );

            // 更新收件箱的最近收件日期
            await db.Inboxes.UpdateAsync(x => x.OrganizationId == SendingItem.OrganizationId,
                               x => x.SetProperty(y => y.LastBeDeliveredDate, DateTime.Now)
                                .SetProperty(y => y.LastSuccessDeliveryDate, DateTime.Now));

            await db.SaveChangesAsync();

            return data;

        }

        /// <summary>
        /// 发件成功后，保存到数据库中
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task SaveOutboxStatus(bool success, string message)
        {
            if (!success) return;
            // 保存到数据库            
            var outbox = await SqlContext.Outboxes.FirstOrDefaultAsync(x => x.Id == Outbox.Id);
            // 更新数据
            outbox.SentTotalToday = Outbox.SentTotalToday;
            await SqlContext.SaveChangesAsync();
        }

        private SendGroupTask _sendGroupTask;
        public void SetSendGroupTask(SendGroupTask sendGroupTask)
        {
            _sendGroupTask = sendGroupTask;
        }

        public int _cooldownMilliseconds = 0;
        /// <summary>
        /// 重新入队
        /// </summary>
        public void Enqueue()
        {
            if (_sendGroupTask == null) return;

            if (_cooldownMilliseconds > 0)
            {
                // 设置冷却
                var _timer = new Timer(_cooldownMilliseconds)
                {
                    AutoReset = false,
                    Enabled = true
                };
                _timer.Elapsed += (object? sender, ElapsedEventArgs e) =>
                {
                    _timer.Stop();
                    // 重新入队
                    _sendGroupTask.Enqueue(this);
                };
            }
            else
            {
                _sendGroupTask.Enqueue(this);
            }
        }
        #endregion

        #region 发送状态变更
        /// <summary>
        /// 设置状态
        /// </summary>
        public void SetStatus()
        {
            // 设置状态为正在发送

        }
        #endregion

        /// <summary>
        /// 转换成发送类
        /// </summary>
        /// <returns></returns>
        public SendMethod ToSendMethod()
        {
            return SendMethodFactory.BuildSendMethod(this);
        }

        #region 重写相等的逻辑
        public override bool Equals(object? obj)
        {
            if (obj is SendItem compareTo) return compareTo.SendingItem.Id.Equals(SendingItem.Id);
            return false;
        }

        public override int GetHashCode()
        {
            return SendingItem.Id.GetHashCode();
        }
        #endregion
    }
}
