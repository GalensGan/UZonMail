using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.EmailSending;
using UZonMailService.Models.SQL.Templates;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.Settings;
using Uamazing.Utils.Json;
using Newtonsoft.Json.Linq;
using UZonMailService.SignalRHubs.SendEmail;
using UZonMailService.Models.SQL.Settings;
using UZonMailService.Models.SQL.Emails;
using Uamazing.Utils.Web.Service;
using Microsoft.AspNetCore.SignalR;
using UZonMailService.SignalRHubs;
using UZonMailService.SignalRHubs.Extensions;
using System.Threading.Tasks;
using UZonMailService.Models.SQL.UserInfos;
using UZonMailService.Utils.Database;

namespace UZonMailService.Services.EmailSending.WaitList
{
    /// <summary>
    /// 一次发件任务
    /// 不同的发件任务包含不同的数据库上下文，保证上下文不重复使用
    /// 包含发件组和收件内容
    /// </summary>
    /// <remarks>
    /// 构造
    /// </remarks>
    public class SendGroupTask
    {
        private readonly UserSendingTaskManager taskManager;
        private readonly SendingGroup sendingGroup;
        private readonly SqlContext sqlContext;
        private readonly UserOutboxesPool outboxesPool;
        private readonly IHubContext<UzonMailHub, IUzonMailClient> hub;
        private readonly ILogger logger;

        private SendGroupTask(UserSendingTaskManager taskManager
        , SendingGroup sendingGroup
        , SqlContext sqlContext
        , UserOutboxesPool outboxesPool
        , IHubContext<UzonMailHub, IUzonMailClient> hub
        , ILogger logger
        )
        {
            this.taskManager = taskManager;
            this.sendingGroup = sendingGroup;
            this.sqlContext = sqlContext;
            this.outboxesPool = outboxesPool;
            this.hub = hub;
            this.logger = logger;
        }

        /// <summary>
        /// 创建一个发件任务
        /// </summary>
        /// <param name="taskManager"></param>
        /// <param name="sendingGroup"></param>
        /// <param name="sqlContext"></param>
        /// <param name="outboxesPool"></param>
        /// <param name="hub"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task<SendGroupTask> Create(UserSendingTaskManager taskManager
        , SendingGroup sendingGroup
        , SqlContext sqlContext
        , UserOutboxesPool outboxesPool
        , IHubContext<UzonMailHub, IUzonMailClient> hub
        , ILogger logger
        )
        {
            SendGroupTask task = new(taskManager, sendingGroup, sqlContext, outboxesPool, hub, logger);
            // 初始化组
            await task.InitSendingGroup();
            return task;
        }

        /// <summary>
        /// 组 id
        /// </summary>
        public long GroupId => sendingGroup.Id;

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool Paused { get; set; } = false;

        /// <summary>
        /// 取消的原因
        /// </summary>
        public string CancelReason { get; set; } = "手动取消";
        /// <summary>
        /// 被取消
        /// </summary>
        public bool Cancelled { get; private set; } = false;

        /// <summary>
        /// 取消后赋予组的状态
        /// </summary>
        public SendingGroupStatus CancelledStatus { get; private set; } = SendingGroupStatus.Cancel;

        #region 初始化
        private SqlContext Db => sqlContext;


        private SendingItemsCounter sendingItemsCounter;

        /// <summary>
        /// 已成功的数量
        /// </summary>
        private int successCount = 0;
        private DateTime _startDate = DateTime.Now;
        public SendingObjectStatus Status { get; private set; } = SendingObjectStatus.Normal;

        /// <summary>
        /// 可用的代理，在发件之前匹配给每个收件箱
        /// </summary>
        private List<UserProxy> _usableProxies = [];

        private async Task<bool> InitSendingGroup()
        {
            // 更新发件组的状态
            sendingGroup.Status = SendingGroupStatus.Sending;
            // 获取成功数、失败数、总数
            var allSendingItems = await Db.SendingItems.Where(x => x.SendingGroupId == sendingGroup.Id)
                .Select(x => new
                {
                    x.Status
                })
                .ToListAsync();
            sendingGroup.TotalCount = allSendingItems.Count;
            sendingGroup.SuccessCount = allSendingItems.Count(x => x.Status == SendingItemStatus.Success);
            sendingGroup.SentCount = allSendingItems.Count(x => x.Status == SendingItemStatus.Success);
            // 开始发送日期
            if (sendingGroup.SendStartDate == DateTime.MinValue)
                sendingGroup.SendStartDate = DateTime.Now;
            await Db.SaveChangesAsync();

            sendingItemsCounter = new SendingItemsCounter(sendingGroup.TotalCount, sendingGroup.SuccessCount, sendingGroup.SentCount);

            // 获取用户所有可用的代理
            _usableProxies = await PullUsableUserProxies();

            // 将新的发件箱添加到发件池中
            var outBoxIds = sendingGroup.Outboxes.Select(x => x.Id).ToList();
            await AddOutboxToPool(outBoxIds);
            return true;
        }

        // 将新的发件箱添加到发件池中,自动去重了
        private async Task AddOutboxToPool(List<long> outBoxIds)
        {
            if (outBoxIds.Count == 0) return;

            var existOutboxes = outboxesPool.GetExistOutboxes(sendingGroup.UserId);
            var newBoxIds = outBoxIds.Except(existOutboxes.Select(x => x.Id)).ToList();
            if (newBoxIds.Count > 0)
            {
                var setting = await UserSettingsFactory.GetUserSettings(Db, sendingGroup.UserId);
                var outboxes = await Db.Outboxes.Where(x => outBoxIds.Contains(x.Id)).ToListAsync();
                foreach (var outbox in outboxes)
                {
                    var newOutboxAddress = outbox.ToOutboxEmailAddress(setting, sendingGroup.Id, sendingGroup.SmtpPasswordSecretKeys);
                    outboxesPool.AddOutbox(sendingGroup.UserId, newOutboxAddress);
                }
            }
            // 对于已经存在的发件箱，更新发送组 id: SendingGroupIds
            var reuseOutboxes = existOutboxes.Where(x => outBoxIds.Contains(x.Id)).ToList();
            foreach (var outbox in reuseOutboxes)
            {
                outbox.SendingGroupIds.Add(sendingGroup.Id);
            }
        }

        /// <summary>
        /// 开始初始化
        /// 有可能会被多次调用
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitSendingItems(List<long>? sendingItemIds)
        {
            // 获取待发件
            List<SendingItem> toSendingItems;
            if (sendingItemIds != null && sendingItemIds.Count > 0)
            {
                toSendingItems = await Db.SendingItems.Where(x => x.SendingGroupId == sendingGroup.Id
                    && (x.Status == SendingItemStatus.Failed || x.Status == SendingItemStatus.Created)
                    && sendingItemIds.Contains(x.Id))
                    .Include(x => x.Attachments)
                    .ToListAsync();
            }
            else
            {
                toSendingItems = await Db.SendingItems.Where(x => x.SendingGroupId == sendingGroup.Id
                     && (x.Status == SendingItemStatus.Failed || x.Status == SendingItemStatus.Created))
                    .Include(x => x.Attachments)
                    .ToListAsync();
            }

            if (toSendingItems.Count == 0) return false;

            // 更新待发件列表中的发件箱
            List<long> outboxIdsFromSendingItems = toSendingItems.Select(x => x.OutBoxId).Where(x => x > 0).ToList();
            await AddOutboxToPool(outboxIdsFromSendingItems);

            // 生成发件列表
            var templates = await PullEmailTemplates();
            // 保存当前发件数量
            sendingItemsCounter.CurrentTotal += toSendingItems.Count;

            foreach (var item in toSendingItems)
            {
                // 将 sendingItem 转换成 sendItem
                var sendItem = item.ToSendItem();
                // 添加数据
                sendItem.BodyData = GetExcelData(item);
                // 生成正文
                sendItem.HtmlBody = GetSendingItemOriginBody(sendItem, templates);
                // 生成主题
                // 若本身有主题，则使用自身的主题
                if (sendItem.BodyData!=null && !string.IsNullOrEmpty(sendItem.BodyData.Subject))
                {
                    sendItem.Subject = sendItem.BodyData.Subject;
                }
                else sendItem.Subject = sendingGroup.GetRandSubject();
                Enqueue(sendItem);
            }

            // 通知用户，任务已开始
            var client = hub.GetUserClient(sendingGroup.UserId);
            if (client != null)
            {
                await client.SendingGroupProgressChanged(new SendingGroupProgressArg(sendingGroup, _startDate)
                {
                    ProgressType = ProgressType.Start
                });
            }

            return true;
        }

        /// <summary>
        /// 获取组中所有的模板
        /// </summary>
        /// <returns></returns>
        private async Task<List<EmailTemplate>> PullEmailTemplates()
        {
            var sharedTemplateIds = sendingGroup.Templates?.Select(x => x.Id).ToList() ?? [];
            List<long> privateTemplateIds = [];
            // 从数据中获取模板
            if (sendingGroup.Data != null && sendingGroup.Data.Count > 0)
            {
                // 有数据
                var templateIdsTemp = sendingGroup.Data.Select(x => x.SelectTokenOrDefault("templateId", 0)).Where(x => x > 0);
                foreach (var item in templateIdsTemp)
                {
                    privateTemplateIds.Add(item);
                }
            }

            var templateIds = new List<long>();
            templateIds.AddRange(sharedTemplateIds);
            templateIds.AddRange(privateTemplateIds);
            templateIds = templateIds.Distinct().ToList();
            if (templateIds.Count == 0) return [];

            var templates = await Db.EmailTemplates
                .Where(x => templateIds.Contains(x.Id) && x.UserId == sendingGroup.UserId)
                .ToListAsync();

            // 对模板进行标注，哪些是通用的模板，哪些是邮件指定的模板
            foreach (var sharedTemplateId in sharedTemplateIds)
            {
                var template = templates.FirstOrDefault(x => x.Id == sharedTemplateId);
                if (template != null)
                {
                    template.Type |= TemplateType.Shared;
                }
            }
            return templates;
        }

        /// <summary>
        /// 获取可用的代理
        /// </summary>
        /// <returns></returns>
        private async Task<List<UserProxy>> PullUsableUserProxies()
        {
            var results = await Db.UserProxies.Where(x => x.UserId == sendingGroup.UserId || x.IsShared)
                .Where(x => x.IsActive)
                .OrderBy(x => x.IsActive)
                .ToListAsync();
            return results;
        }

        /// <summary>
        /// 获取 excel 数据
        /// </summary>
        /// <param name="sendingItem"></param>
        /// <returns></returns>
        private SendingItemExcelData? GetExcelData(SendingItem sendingItem)
        {
            // 原版：从组中获取数据
            //if (sendingItem.IsSendingBatch) return null;
            //if (sendingGroup.Data == null || sendingGroup.Data.Count == 0) return null;
            //// 查找
            //var data = sendingGroup.Data.FirstOrDefault(x => x.SelectTokenOrDefault("inbox", string.Empty) == sendingItem.Inboxes[0].Email);
            //if (data == null) return null;
            //return new SendingItemExcelData(data as JObject);

            // 新版：从发送项中获取数据
            // 不兼容新版本
            return new SendingItemExcelData(sendingItem.Data);
        }

        /// <summary>
        /// 获取原始发件内容
        /// 变量未经过处理
        /// </summary>
        /// <param name="sendItem"></param>
        /// <param name="templates"></param>
        /// <returns></returns>
        private string GetSendingItemOriginBody(SendItem sendItem, List<EmailTemplate> templates)
        {
            if (sendItem.IsSendingBatch)
            {
                // 说明是批量发送
                if (!string.IsNullOrEmpty(sendingGroup.Body)) return sendingGroup.Body;

                // 若本身有模板，则返回自己的模板
                if (sendItem.SendingItem.EmailTemplateId > 0)
                {
                    return templates.FirstOrDefault(x => x.Id == sendingGroup.Id)?.Content ?? string.Empty;
                }

                // 否则返回第一个公用模板
                return templates.FirstOrDefault(x => x.Type.HasFlag(TemplateType.Shared))?.Content ?? string.Empty;
            }

            // 非批量发送
            // 当有用户数据时
            if (sendItem.BodyData != null)
            {
                // 判断是否有 body
                if (!string.IsNullOrEmpty(sendItem.BodyData.Body)) return sendItem.BodyData.Body;

                // 判断是否有模板 id 或者模板名称
                if (sendItem.BodyData.TemplateId > 0)
                {
                    var template = templates.FirstOrDefault(x => x.Id == sendItem.BodyData.TemplateId);
                    if (template != null) return template.Content;
                }

                // 判断是否有模板名称
                if (!string.IsNullOrEmpty(sendItem.BodyData.TemplateName))
                {
                    var template = templates.FirstOrDefault(x => x.Name == sendItem.BodyData.TemplateName);
                    if (template != null) return template.Content;
                }
            }

            // 没有数据时，优先使用组中的 body
            if (!string.IsNullOrEmpty(sendingGroup.Body)) return sendingGroup.Body;

            // 返回随机模板
            var sharedTemplates = templates.Where(x => x.Type.HasFlag(TemplateType.Shared)).ToList();
            if (sharedTemplates.Count == 0) return string.Empty;

            return sharedTemplates[new Random().Next(sharedTemplates.Count)].Content;
        }
        #endregion

        /// <summary>
        /// 发件列表
        /// </summary>
        private Queue<SendItem> _sendItemsQueue = new();

        /// <summary>
        /// 获取发件项
        /// </summary>
        /// <returns></returns>
        public async Task<SendItem?> GetSendItem()
        {
            // 暂停时，不发件
            if (Paused) return null;

            if (!_sendItemsQueue.TryDequeue(out var sendItem)) return null;

            // 获取发件箱
            OutboxEmailAddress? outboxEmailAddress = outboxesPool.GetOutbox(sendingGroup.UserId, sendingGroup.Id, sendItem.SendingItem.OutBoxId, out int status);
            if (status == 0)
            {
                // 说明未匹配到发件箱，需要将当前发件移除
                await EmailItemSendCompleted(false, "未匹配到发件箱");
                return null;
            }

            if (outboxEmailAddress == null)
            {
                // 重新放回去
                _sendItemsQueue.Enqueue(sendItem);
                return null;
            }

            // 为 sendItem 动态赋值
            // 赋予发件箱
            sendItem.Outbox = outboxEmailAddress;

            // 赋予发件任务
            sendItem.SetSendGroupTask(this);
            // 添加代理
            // 代理与发件箱进行匹配            
            sendItem.ProxyInfo = GetMatchedProxy(outboxEmailAddress, sendItem.SendingItem.ProxyId);

            // 赋予其它 DI 参数
            sendItem.Logger = logger;
            sendItem.Hub = hub;
            sendItem.Db = Db;

            // 推送开始发件
            var client = hub.GetUserClient(sendingGroup.UserId);
            client?.SendingItemStatusChanged(new SendingItemStatusChangedArg(sendItem.SendingItem)
            {
                Status = SendingItemStatus.Sending
            });

            return sendItem;
        }

        /// <summary>
        /// 匹配代理
        /// </summary>
        /// <param name="outbox"></param>
        /// <param name="proxyId"></param>
        /// <returns></returns>
        private ProxyInfo? GetMatchedProxy(OutboxEmailAddress outbox, long proxyId = 0)
        {
            if (proxyId == 0) proxyId = outbox.ProxyId;

            // 优先使用发件箱的代理
            if (proxyId > 0)
            {
                return _usableProxies.FirstOrDefault(x => x.Id == proxyId)?.ToProxyInfo();
            }

            // 匹配代理
            return _usableProxies.FirstOrDefault(x => x.IsMatch(outbox.Email))?.ToProxyInfo();
        }

        /// <summary>
        /// 外部调用，添加发件项
        /// </summary>
        /// <param name="sendItem"></param>
        /// <returns></returns>
        public bool Enqueue(SendItem? sendItem)
        {
            if (sendItem == null) return false;
            if (_sendItemsQueue.Contains(sendItem)) return true;
            _sendItemsQueue.Enqueue(sendItem);
            return true;
        }

        /// <summary>
        /// 邮件发送完成
        /// 只储存 邮件组 数据，具体的每次发件数据在 SendItem 中处理
        /// 只有失败或者成功才会触发，重发时不会触发
        /// </summary>
        /// <param name="success">是否发送成功</param>
        public async Task EmailItemSendCompleted(bool success, string message)
        {
            // 只有已经发送成功或者不重试了，才读入进度
            sendingItemsCounter.IncreaseSuccessCount(success);

            // 向数据库中保存状态
            var newSendingGroup = await Db.SendingGroups.FirstAsync(x => x.Id == sendingGroup.Id);
            newSendingGroup.SuccessCount = sendingItemsCounter.TotalSuccessCount;
            newSendingGroup.TotalCount = sendingGroup.TotalCount;
            newSendingGroup.SentCount = sendingItemsCounter.TotalSentCount;
            newSendingGroup.LastMessage = message;

            // 向用户推送发送组的进度
            var client = hub.GetUserClient(sendingGroup.UserId);
            if (client != null)
            {
                // 推送发送组进度
                await client.SendingGroupProgressChanged(new SendingGroupProgressArg(newSendingGroup, _startDate));
            }

            // 设置当前任务状态
            if (sendingItemsCounter.CurrentTotal == sendingItemsCounter.CurrentSentCount)
            {
                // 说明已经发完了
                Status = SendingObjectStatus.ShouldDispose;

                // 推送任务完成
                // 推送邮件组发送结束进度
                if (client != null)
                {
                    newSendingGroup.Status = SendingGroupStatus.Finish;
                    newSendingGroup.SendEndDate = DateTime.Now;
                    await client.SendingGroupProgressChanged(new SendingGroupProgressArg(newSendingGroup, _startDate)
                    {
                        Total = sendingItemsCounter.CurrentTotal,
                        Current = sendingItemsCounter.CurrentSentCount,
                        SuccessCount = sendingItemsCounter.CurrentSuccessCount,
                        ProgressType = ProgressType.End
                    });
                }
            }

            // 被取消
            if (Cancelled)
            {
                Status = SendingObjectStatus.ShouldDispose;
                newSendingGroup.Status = CancelledStatus;
                newSendingGroup.SendEndDate = DateTime.Now;
                newSendingGroup.LastMessage = CancelReason;

                if (client != null)
                {
                    newSendingGroup.SendEndDate = DateTime.Now;
                    await client.SendingGroupProgressChanged(new SendingGroupProgressArg(newSendingGroup, _startDate)
                    {
                        ProgressType = ProgressType.End
                    });
                }
            }
            await Db.SaveChangesAsync();

            // 调用外部回调
            await taskManager.EmailItemSendCompleted(success);
        }

        /// <summary>
        /// 标记为取消
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task MarkCancelled(string message = "手动取消", SendingGroupStatus sendingGroupStatus = SendingGroupStatus.Cancel)
        {
            Cancelled = true;
            CancelReason = message;
            CancelledStatus = sendingGroupStatus;

            // 通知前端发送结束
            var client = hub.GetUserClient(sendingGroup.UserId);
            if (client != null)
            {
                await client.SendingGroupProgressChanged(new SendingGroupProgressArg(sendingGroup, _startDate)
                {
                    ProgressType = ProgressType.End
                });
            }
            Status = SendingObjectStatus.ShouldDispose;
        }
    }
}
