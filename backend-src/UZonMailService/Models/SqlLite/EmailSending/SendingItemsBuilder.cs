using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Xml.Linq;
using Uamazing.Utils.Json;
using UZonMailService.Models.SqlLite.Settings;

namespace UZonMailService.Models.SqlLite.EmailSending
{
    /// <summary>
    /// 用于生成发送项
    /// </summary>
    /// <param name="group"></param>
    /// <param name="userSetting"></param>
    public class SendingItemsBuilder(SendingGroup group, UserSetting userSetting)
    {
        /// <summary>
        /// 批量发件时的大小
        /// </summary>
        private int _batchSize = userSetting.MaxSendingBatchSize;

        public List<SendingItem> Build()
        {
            // 获取收件箱
            List<EmailAddress> inboxes = GetInboxes();

            // 根据收件箱生成发件项
            return GenerateSendingItems(inboxes);
        }

        /// <summary>
        /// 获取收件箱
        /// 数据关键字为: inbox,inboxName
        /// 会根据 email 去重
        /// </summary>
        /// <returns></returns>
        private List<EmailAddress> GetInboxes()
        {
            if (group.Data == null) return group.Inboxes;

            // 若有数据，从数据中添加
            List<EmailAddress> inboxes = [];
            inboxes.AddRange(group.Inboxes);

            foreach (var data in group.Data)
            {
                var inbox = data.SelectTokenOrDefault("inbox", string.Empty);
                if (string.IsNullOrEmpty(inbox)) continue;
                // 判断是否重复
                if (inboxes.Any(x => x.Email == inbox)) continue;

                // 查找收件箱名称
                var inboxName = data.SelectTokenOrDefault("inboxName", string.Empty);
                inboxes.Add(new EmailAddress()
                {
                    Email = inbox,
                    Name = inboxName
                });
            }
            return inboxes.DistinctBy(x=>x.Email).ToList();
        }

        /// <summary>
        /// 开始生成发送项
        /// 当满足以下条件时，对收件人进行合并处理
        /// 1-发件人只有一个
        /// 2-没有数据
        /// 3-只有一个模板或者没有模板
        /// </summary>
        /// <param name="inboxes"></param>
        /// <returns></returns>
        private List<SendingItem> GenerateSendingItems(List<EmailAddress> inboxes)
        {
            if (group.SendBatch
                && group.Outboxes.Count == 1
                && (group.Data == null || group.Data.Count == 0)
                && (group.Templates == null || group.Templates.Count <= 1))
            {
                // 批量发送时，设置按最大批量进行分割
                // 批量数包含抄送和密送
                int actualBatchSize = _batchSize;
                if (group.CcBoxes != null)
                {
                    actualBatchSize -= group.CcBoxes.Count;
                }
                if (group.BccBoxes != null)
                {
                    actualBatchSize -= group.BccBoxes.Count;
                }
                actualBatchSize = Math.Max(1, actualBatchSize);

                // 分批发送
                List<SendingItem> sendingItemsResult = [];
                int total = 0;
                while (total < inboxes.Count)
                {
                    var inboxesTemp = inboxes.Skip(total).Take(actualBatchSize).ToList();
                    total += inboxesTemp.Count;

                    var sendingItem = new SendingItem()
                    {
                        OutBoxId = group.Outboxes[0].Id,
                        FromEmail = group.Outboxes[0].Email,
                        SendingGroupId = group.Id,
                        UserId = group.UserId,
                        Inboxes = inboxesTemp,
                        CC = group.CcBoxes,
                        BCC = group.BccBoxes,
                        Attachments = group.Attachments,
                        Status = SendingItemStatus.Created,
                        IsSendingBatch = true
                    };
                    sendingItemsResult.Add(sendingItem);
                }
                return sendingItemsResult;
            }

            // 非合并数据的情况
            Dictionary<string, SendingItemExcelData> rowData = [];
            if (group.Data != null)
            {
                foreach (var data in group.Data)
                {
                    var row = new SendingItemExcelData(data as JObject);
                    rowData.Add(row.Inbox, row);
                }
            }

            List<SendingItem> sendingItems = [];
            foreach (var inbox in inboxes)
            {
                var sendingItem = new SendingItem()
                {
                    SendingGroupId = group.Id,
                    UserId = group.UserId,
                    // 对于携带变量的情况，仅支持一对一发件
                    Inboxes = [inbox],
                    CC = group.CcBoxes,
                    BCC = group.BccBoxes,
                    Attachments = group.Attachments,
                    Status = SendingItemStatus.Created
                };
                // 在发送时，才会设置模板
                sendingItems.Add(sendingItem);

                // 从数据中获取相关数据
                if (!rowData.TryGetValue(inbox.Email, out var row))
                {
                    continue;
                }

                // 设置发件箱
                sendingItem.OutBoxId = row.OutboxId;
                sendingItem.FromEmail = row.Outbox;
                if (row.OutboxId > 0 && row.ProxyId > 0)
                {
                    // 代理 Id
                    sendingItem.ProxyId = row.ProxyId;
                }

                // 设置数据
                // 抄送
                if (row.CC != null && row.CC.Count > 0)
                {
                    sendingItem.CC ??= [];

                    foreach (var cc in row.CC)
                    {
                        // 如果不存在，则添加
                        if (sendingItem.CC.Any(x => x.Email == cc)) continue;
                        sendingItem.CC.Add(new EmailAddress()
                        {
                            Email = cc,
                            Name = cc
                        });
                    }
                }

                // 密送
                if (row.BCC != null && row.BCC.Count > 0)
                {
                    sendingItem.BCC ??= [];

                    foreach (var bcc in row.BCC)
                    {
                        // 如果不存在，则添加
                        if (sendingItem.BCC.Any(x => x.Email == bcc)) continue;
                        sendingItem.BCC.Add(new EmailAddress()
                        {
                            Email = bcc,
                            Name = bcc
                        });
                    }
                }

                // 指定模板
                if (row.TemplateId > 0)
                {
                    sendingItem.EmailTemplateId = row.TemplateId;
                }

                // 覆盖正文
                // 正文和模板在 SendGroupTask.Init() 中设置
            }

            return sendingItems;
        }
    }
}
