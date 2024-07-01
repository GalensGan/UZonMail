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
using UZonMailService.Models.SQL.MultiTenant;
using UZonMailService.Utils.Database;
using log4net;
using UZonMailService.Services.EmailSending.Models;

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
    public class SendingGroupTask
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SendingGroupTask));
        private readonly object _lockObject = new();

        private SendingGroupTask(long sendingGroupId, List<string> smtpPasswordSecretKeys)
        {
            SendingGroupId = sendingGroupId;
            SmtpPasswordSecretKeys = smtpPasswordSecretKeys;
        }

        /// <summary>
        /// 创建一个发件任务
        /// </summary>
        /// <param name="scopeServices"></param>
        /// <param name="sendingGroupId"></param>
        /// <param name="smtpPasswordSecretKeys"></param>
        /// <returns></returns>
        public static async Task<SendingGroupTask?> Create(ScopeServices scopeServices
        , long sendingGroupId
        , List<string> smtpPasswordSecretKeys
        )
        {
            SendingGroupTask task = new(sendingGroupId, smtpPasswordSecretKeys);
            // 初始化组
            if (!await task.InitSendingGroup(scopeServices)) return null;

            return task;
        }

        #region 属性
        /// <summary>
        /// 组 id
        /// </summary>
        public long SendingGroupId { get; private set; }

        /// <summary>
        /// 通过组 id 获取的组
        /// </summary>
        private SendingGroup _sendingGroup;

        /// <summary>
        /// 密钥
        /// </summary>
        public List<string> SmtpPasswordSecretKeys { get; private set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        public long UserId { get; private set; }

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

        /// <summary>
        /// 发送计数器
        /// </summary>
        private SendingItemsCounter _sendingItemsCounter;

        /// <summary>
        /// 没有发件箱的发件项
        /// </summary>
        private ConcurrentBag<SendItemMeta> _sendingItemsWithoutOutboxBag = [];

        /// <summary>
        /// 具有发件箱的发件项
        /// </summary>
        private ConcurrentDictionary<string, ConcurrentBag<SendItemMeta>> _sendingItemsWithOutboxDic = new();

        /// <summary>
        /// 已成功的数量
        /// </summary>
        private DateTime _startDate = DateTime.Now;
        public SendingObjectStatus Status { get; private set; } = SendingObjectStatus.Normal;
        #endregion


        /// <summary>
        /// 初始化发件组信息
        /// </summary>
        /// <param name="scopeServices"></param>
        /// <returns></returns>
        private async Task<bool> InitSendingGroup(ScopeServices scopeServices)
        {
            // 获取完整的邮件组
            var sendingGroup = await scopeServices.SqlContext.SendingGroups
                .Where(x => x.Id == SendingGroupId)
                .Include(x => x.Outboxes)
                .Include(x => x.Templates)
                .Select(x => new SendingGroup()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Outboxes = x.Outboxes,
                    Templates = x.Templates,
                    SendStartDate = x.SendStartDate,
                    Status = x.Status,
                })
                .FirstOrDefaultAsync();
            if (sendingGroup == null)
            {
                _logger.Error($"发件组 {SendingGroupId} 不存在");
                return false;
            }
            // 正在发送时，不添加
            if (sendingGroup.Status == SendingGroupStatus.Sending)
            {
                _logger.Error($"发件组 {SendingGroupId} 正在发送中");
                return false;
            }
            _sendingGroup = sendingGroup;

            // 更新用户 id
            UserId = sendingGroup.UserId;

            // 更新发件组的状态
            sendingGroup.Status = SendingGroupStatus.Sending;
            // 获取成功数、失败数、总数
            var allSendingItems = await scopeServices.SqlContext.SendingItems.AsNoTracking()
                .Where(x => x.SendingGroupId == sendingGroup.Id)
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
            await scopeServices.SqlContext.SaveChangesAsync();

            _sendingItemsCounter = new SendingItemsCounter(sendingGroup.TotalCount, sendingGroup.SuccessCount, sendingGroup.SentCount);

            // 将新的公用发件箱添加到发件池中
            await AddOutboxToPool(scopeServices, sendingGroup.Outboxes);
            return true;
        }

        // 将新的发件箱添加到发件池中,自动去重了
        private async Task AddOutboxToPool(ScopeServices scopeServices, List<Outbox> outboxes)
        {
            if (outboxes.Count == 0) return;
            var outboxesPoolManager = scopeServices.ServiceProvider.GetRequiredService<UserOutboxesPoolManager>();

            var outboxAddresses = outboxes.ConvertAll(x => new OutboxEmailAddress(x, SendingGroupId, SmtpPasswordSecretKeys, OutboxEmailAddressType.Shared));
            foreach (var outbox in outboxAddresses)
            {
                await outboxesPoolManager.AddOutbox(outbox);
            }
        }


        /// <summary>
        /// 开始初始化
        /// 有可能会被多次调用
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitSendingItems(ScopeServices scopeServices, List<long>? sendingItemIds)
        {
            // 获取待发件
            var dbSet = scopeServices.SqlContext.SendingItems.AsNoTracking()
                .Where(x => x.SendingGroupId == SendingGroupId)
                .Where(x => x.Status == SendingItemStatus.Failed || x.Status == SendingItemStatus.Created);
            if (sendingItemIds != null && sendingItemIds.Count > 0)
            {
                dbSet = dbSet.Where(x => sendingItemIds.Contains(x.Id));
            }
            List<SendingItem> toSendingItems = await dbSet.Select(x => new SendingItem() { Id = x.Id, OutBoxId = x.OutBoxId }).ToListAsync();
            HashSet<long> outboxIds = toSendingItems.Select(x => x.OutBoxId).ToHashSet();
            var outboxes = await scopeServices.SqlContext.Outboxes.Where(x => outboxIds.Contains(x.Id)).ToListAsync();
            // 新增特定发件箱
            var outboxesPoolManager = scopeServices.ServiceProvider.GetRequiredService<UserOutboxesPoolManager>();
            foreach (var outbox in outboxes)
            {
                // 获取收件项 Id
                var sendingItemIdsTemp = toSendingItems.Where(x => x.OutBoxId == outbox.Id).Select(x => x.Id).ToList();
                var outboxAddress = new OutboxEmailAddress(outbox, SendingGroupId, SmtpPasswordSecretKeys, OutboxEmailAddressType.Specific, sendingItemIdsTemp);
                await outboxesPoolManager.AddOutbox(outboxAddress);
            }

            // 更新待发件列表
            var toSendingItemMetasWithoutOutbox = toSendingItems.Where(x => x.OutBoxId == 0).Select(x => new SendItemMeta(x)).ToList();
            foreach (var item in toSendingItemMetasWithoutOutbox)
            {
                _sendingItemsWithoutOutboxBag.Add(item);
            }
            var toSendingItemMetasWithOutbox = toSendingItems.Where(x => x.OutBoxId > 0).Select(x =>
            {
                var outbox = outboxes.FirstOrDefault(o => o.Id == x.OutBoxId);
                if (outbox == null) return null;
                return new SendItemMeta(x, outbox.Email);
            }).ToList();

            // 更新当前发件组的总数
            _sendingItemsCounter.IncreaseTotalCount(toSendingItems.Count);

            // 将发件项修改为发送中
            if (sendingItemIds != null && sendingItemIds.Count > 0)
            {
                await scopeServices.SqlContext.SendingItems.UpdateAsync(x => sendingItemIds.Contains(x.Id),
                    x => x.SetProperty(y => y.Status, SendingItemStatus.Pending));
            }
            else
            {
                await scopeServices.SqlContext.SendingItems.UpdateAsync(x => x.SendingGroupId == SendingGroupId,
                                       x => x.SetProperty(y => y.Status, SendingItemStatus.Pending));
            }

            // 通知用户，任务已开始
            var hub = scopeServices.HubClient;
            var client = hub.GetUserClient(UserId);
            if (client != null)
            {
                await client.SendingGroupProgressChanged(new SendingGroupProgressArg(_sendingGroup, _startDate)
                {
                    ProgressType = ProgressType.Start
                });
            }


            // 更新待发件列表中的发件箱
            //List<long> outboxIdsFromSendingItems = toSendingItems.Select(x => x.OutBoxId).Where(x => x > 0).ToList();
            //await AddOutboxToPool(outboxIdsFromSendingItems);

            //// 生成发件列表
            //var templates = await PullEmailTemplates();
            //// 保存当前发件数量
            //_sendingItemsCounter.CurrentTotal += toSendingItems.Count;

            //foreach (var item in toSendingItems)
            //{
            //    // 将 sendingItem 转换成 sendItem
            //    var sendItem = item.ToSendItem();
            //    // 添加数据
            //    sendItem.BodyData = GetExcelData(item);
            //    // 生成正文
            //    sendItem.HtmlBody = GetSendingItemOriginBody(sendItem, templates);
            //    // 生成主题
            //    // 若本身有主题，则使用自身的主题
            //    if (sendItem.BodyData != null && !string.IsNullOrEmpty(sendItem.BodyData.Subject))
            //    {
            //        sendItem.Subject = sendItem.BodyData.Subject;
            //    }
            //    else sendItem.Subject = sendingGroup.GetRandSubject();
            //    Enqueue(sendItem);
            //}

            //// 通知用户，任务已开始
            //var client = hub.GetUserClient(sendingGroup.UserId);
            //if (client != null)
            //{
            //    await client.SendingGroupProgressChanged(new SendingGroupProgressArg(sendingGroup, _startDate)
            //    {
            //        ProgressType = ProgressType.Start
            //    });
            //}

            return true;
        }

        /// <summary>
        /// 获取组中所有的模板
        /// </summary>
        /// <returns></returns>
        private async Task<List<EmailTemplate>> PullEmailTemplates()
        {
            var sharedTemplateIds = _sendingGroup.Templates?.Select(x => x.Id).ToList() ?? [];
            List<long> privateTemplateIds = [];
            // 从数据中获取模板
            if (_sendingGroup.Data != null && _sendingGroup.Data.Count > 0)
            {
                // 有数据
                var templateIdsTemp = _sendingGroup.Data.Select(x => x.SelectTokenOrDefault("templateId", 0)).Where(x => x > 0);
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
        private async Task<List<UserProxy>> PullUsableUserProxies(ScopeServices scopeServices)
        {
            var results = await scopeServices.SqlContext.UserProxies.Where(x => x.UserId == UserId || x.IsShared)
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

        /// <summary>
        /// 从数据库获取 SendingItem，然后转换成 SendItem
        /// </summary>
        /// <param name="outbox"></param>
        /// <returns></returns>
        private async Task<SendingItem> GetSendItemFromSql(ScopeServices scopeServices, OutboxEmailAddress outbox)
        {
            // 先发指定项
            var sendingItemId = 0L;
            if (outbox.Type.HasFlag(OutboxEmailAddressType.Specific))
            {
                lock (outbox.SendingItemIdsLock)
                {
                    if (outbox.SendingItemIds.Count > 0)
                    {
                        sendingItemId = outbox.SendingItemIds.First();
                        outbox.SendingItemIds.Remove(sendingItemId);
                    }
                }
            }

            SendingItem? sendingItem = null;
            if (sendingItemId > 0)
            {
                // 获取指定的发件项
                sendingItem = await scopeServices.SqlContext.SendingItems.Where(x => x.Id == sendingItemId)
                    .FirstOrDefaultAsync();
            }
            else
            {
                // 查找未发送的邮件
            }
        }

        /// <summary>
        /// 获取发件项
        /// </summary>
        /// <returns></returns>
        public async Task<SendItem?> GetSendItem(ScopeServices scopeServices, OutboxEmailAddress outbox)
        {
            // 判断是否为当前组对应的发件箱
            if (!outbox.SendingGroupIds.Contains(SendingGroupId)) return null;
            // 暂停时，不发件
            if (Paused) return null;

            var sendingItem = await GetSendItemFromSql(scopeServices, outbox);

            if (!_sendItemsQueue.TryDequeue(out var sendItem)) return null;
            // 附加执行过程的信息
            // 赋予发件任务
            sendItem.SetSendGroupTask(this);
            // 赋予其它 DI 参数
            sendItem.Logger = _logger;
            sendItem.Hub = hub;
            sendItem.SqlContext = sqlContext;

            // 获取发件箱
            OutboxEmailAddress? outboxEmailAddress = outboxesPool.GetOutbox(sendingGroup.UserId, sendingGroup.Id, sendItem.SendingItem.OutBoxId, out int status);
            if (status == 0)
            {
                // 更新邮件状态
                await sqlContext.SendingItems.UpdateAsync(x => x.Id == sendItem.SendingItem.Id, x => x.SetProperty(y => y.Status, SendingItemStatus.Failed)
                    .SetProperty(y => y.SendDate, DateTime.Now)
                    .SetProperty(y => y.SendResult, "未匹配到发件箱")
                );
                // 说明未匹配到发件箱，需要将当前发件移除
                await EmailItemSendCompleted(new SendResult(sendItem, false, "未匹配到发件箱"));
                return null;
            }

            if (outboxEmailAddress == null)
            {
                // 重新放回去
                _sendItemsQueue.Enqueue(sendItem);
                return null;
            }

            // 判断收件箱是否牌冷却中
            // 从数据库中判断
            var (isCooling, message) = await IsInboxICooling(sendItem.Inboxes.ConvertAll(x => x.Id));
            if (isCooling)
            {
                // 更新邮件状态
                await sqlContext.SendingItems.UpdateAsync(x => x.Id == sendItem.SendingItem.Id, x => x.SetProperty(y => y.Status, SendingItemStatus.Failed)
                    .SetProperty(y => y.SendDate, DateTime.Now)
                    .SetProperty(y => y.SendResult, message)
                );
                await EmailItemSendCompleted(new SendResult(sendItem, false, message));
                return null;
            }

            // 为 sendItem 动态赋值
            // 赋予发件箱
            sendItem.Outbox = outboxEmailAddress;
            // 赋予回信人
            sendItem.ReplyToEmails = outboxEmailAddress.ReplyToEmails;
            if (sendItem.ReplyToEmails.Count == 0)
            {
                // 使用全局回复
                var userSetting = await UserSettingsCache.GetUserSettings(sqlContext, sendingGroup.UserId);
                sendItem.ReplyToEmails = userSetting.ReplyToEmailsList;
            }

            // 添加代理
            // 代理与发件箱进行匹配            
            sendItem.ProxyInfo = GetMatchedProxy(outboxEmailAddress, sendItem.SendingItem.ProxyId);


            // 推送开始发件
            var client = hub.GetUserClient(sendingGroup.UserId);
            client?.SendingItemStatusChanged(new SendingItemStatusChangedArg(sendItem.SendingItem)
            {
                Status = SendingItemStatus.Sending
            });

            return sendItem;
        }

        private async Task<Tuple<bool, string>> IsInboxICooling(List<long> inboxIds)
        {
            // 获取收件箱信息
            var inboxes = await Db.Inboxes.Where(x => inboxIds.Contains(x.Id))
                .ToListAsync();
            if (inboxes.Count == 0) return Tuple.Create(false, "");

            // 自身有冷却设置
            var inboxesWithSelfCooling = inboxes.Where(x => x.MinInboxCooldownHours >= 0).ToList();
            var coolingInbox = inboxesWithSelfCooling.FirstOrDefault(x => x.LastSuccessDeliveryDate.AddHours(x.MinInboxCooldownHours) > DateTime.Now);
            if (coolingInbox != null)
            {
                return Tuple.Create(true, $"收件箱 {coolingInbox.Email} 正在冷却中");
            }

            // 从缓存中读取设置数据
            var setting = await UserSettingsCache.GetUserSettings(sqlContext, sendingGroup.UserId);
            if (setting.MinInboxCooldownHours <= 0) return Tuple.Create(false, "");

            var inboxesWhithGlobalCooling = inboxes.Where(x => x.MinInboxCooldownHours < 0).ToList();
            coolingInbox = inboxesWhithGlobalCooling.FirstOrDefault(x => x.LastSuccessDeliveryDate.AddHours(setting.MinInboxCooldownHours) > DateTime.Now);
            if (coolingInbox != null)
            {
                return Tuple.Create(true, $"收件箱 {coolingInbox.Email} 正在冷却中");
            }

            // 从数据库中判断
            return Tuple.Create(false, "");
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
        /// <param name="sendCompleteResult">发送结果</param>
        public async Task EmailItemSendCompleted(SendResult sendCompleteResult)
        {
            // 只有已经发送成功或者不重试了，才读入进度
            _sendingItemsCounter.IncreaseSuccessCount(sendCompleteResult.Ok);
            var sqlContext = sendCompleteResult.SqlContext;

            // 向数据库中保存状态
            var newSendingGroup = await sqlContext.SendingGroups.FirstAsync(x => x.Id == sendingGroup.Id);
            newSendingGroup.SuccessCount = _sendingItemsCounter.TotalSuccessCount;
            newSendingGroup.TotalCount = sendingGroup.TotalCount;
            newSendingGroup.SentCount = _sendingItemsCounter.TotalSentCount;
            newSendingGroup.LastMessage = sendCompleteResult.Message;

            // 向用户推送发送组的进度
            var client = hub.GetUserClient(sendingGroup.UserId);
            if (client != null)
            {
                // 推送发送组进度
                await client.SendingGroupProgressChanged(new SendingGroupProgressArg(newSendingGroup, _startDate));
            }

            // 设置当前任务状态
            if (_sendingItemsCounter.CurrentTotal == _sendingItemsCounter.CurrentSentCount)
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
                        Total = _sendingItemsCounter.CurrentTotal,
                        Current = _sendingItemsCounter.CurrentSentCount,
                        SuccessCount = _sendingItemsCounter.CurrentSuccessCount,
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
            await sqlContext.SaveChangesAsync();

            // 调用外部回调
            await taskManager.EmailItemSendCompleted(sendCompleteResult);
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
