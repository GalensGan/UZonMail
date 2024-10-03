using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using UZonMail.Core.Services.EmailSending.OutboxPool;
using UZonMail.Core.Services.EmailSending.Sender;
using UZonMail.Core.Services.Settings;
using UZonMail.Core.SignalRHubs.SendEmail;
using UZonMail.Core.SignalRHubs.Extensions;
using UZonMail.Core.Utils.Database;
using log4net;
using UZonMail.Core.Services.EmailSending.Pipeline;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.DB.SQL.Emails;
using UZonMail.DB.SQL.Settings;
using UZonMail.DB.SQL.Templates;
using UZonMail.Core.Database.SQL.EmailSending;
using UZonMail.DB.SQL;
using UZonMail.Utils.Json;

namespace UZonMail.Core.Services.EmailSending.WaitList
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
        public static async Task<SendingGroupTask?> Create(SendingContext scopeServices
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
        /// 发送计数器
        /// </summary>
        private SendingItemsCounter _sendingItemsCounter;

        /// <summary>
        /// 发件项数据
        /// 此处只保存自由发件的项
        /// 若指定发件箱，数据id则保存在 outbox 中
        /// </summary>
        private ConcurrentBag<SendItemMeta> _sendingItemMetas = [];

        /// <summary>
        /// 可用的代理，直接保存在内存中
        /// </summary>
        private List<OrganizationProxy> _usableProxies = [];
        /// <summary>
        /// 当前发件箱可用的的所有的模板
        /// 由于模板不多，因此直接保存在内存中
        /// </summary>
        private List<EmailTemplate> _allTemplates = [];

        /// <summary>
        /// 是否应该释放
        /// </summary>
        public bool ShouldDispose => _sendingItemMetas.Count == 0 && _sendingItemsCounter.RunningCount == 0;

        /// <summary>
        /// 已成功的数量
        /// </summary>
        private DateTime _startDate = DateTime.Now;
        #endregion


        /// <summary>
        /// 初始化发件组信息
        /// </summary>
        /// <param name="scopeServices"></param>
        /// <returns></returns>
        private async Task<bool> InitSendingGroup(SendingContext scopeServices)
        {
            // 获取完整的邮件组
            var sendingGroup = await scopeServices.SqlContext.SendingGroups
                .AsNoTracking()
                .Where(x => x.Id == SendingGroupId)
                .Include(x => x.Outboxes)
                .Include(x => x.Templates)
                //.Select(x => new SendingGroup()
                //{
                //    Id = x.Id,
                //    UserId = x.UserId,
                //    Outboxes = x.Outboxes,
                //    Templates = x.Templates,
                //    SendStartDate = x.SendStartDate,
                //    Status = x.Status,
                //    Subjects = x.Subjects
                //})
                .FirstOrDefaultAsync();
            if (sendingGroup == null)
            {
                _logger.Error($"发件组 {SendingGroupId} 不存在");
                return false;
            }

            // 将收件箱重置为空，方便垃圾回收
            sendingGroup.Inboxes = [];

            // 正在发送时，不添加
            if (sendingGroup.Status == SendingGroupStatus.Sending)
            {
                _logger.Error($"发件组 {SendingGroupId} 正在发送中");
                return false;
            }
            _sendingGroup = sendingGroup;

            // 更新用户 id
            UserId = sendingGroup.UserId;

            // 获取成功数、失败数、总数
            var allSendingItems = await scopeServices.SqlContext.SendingItems.AsNoTracking()
                .Where(x => x.SendingGroupId == sendingGroup.Id)
                .Select(x => new
                {
                    x.Status
                })
                .ToListAsync();

            // 更新发件组的状态
            sendingGroup.Status = SendingGroupStatus.Sending;
            sendingGroup.TotalCount = allSendingItems.Count;
            sendingGroup.SuccessCount = allSendingItems.Count(x => x.Status == SendingItemStatus.Success);
            sendingGroup.SentCount = allSendingItems.Count(x => x.Status == SendingItemStatus.Success);

            // 开始发送日期
            if (sendingGroup.SendStartDate == DateTime.MinValue)
                sendingGroup.SendStartDate = DateTime.Now;
            // 更新组正状态
            await scopeServices.SqlContext.SendingGroups.UpdateAsync(x => x.Id == SendingGroupId,
                               x => x.SetProperty(y => y.Status, SendingGroupStatus.Sending)
                                   .SetProperty(y => y.SendStartDate, sendingGroup.SendStartDate)
                                   .SetProperty(y => y.TotalCount, sendingGroup.TotalCount)
                                   .SetProperty(y => y.SuccessCount, sendingGroup.SuccessCount)
                                   .SetProperty(y => y.SentCount, sendingGroup.SentCount));

            _sendingItemsCounter = new SendingItemsCounter(sendingGroup.TotalCount, sendingGroup.SuccessCount, sendingGroup.SentCount);

            // 将新的公用发件箱添加到发件池中
            await AddSharedOutboxToPool(scopeServices, sendingGroup.Outboxes);
            if (sendingGroup.OutboxGroups != null && sendingGroup.OutboxGroups.Count > 0)
            {
                var outboxGroupIds = sendingGroup.OutboxGroups.Select(x => x.Id).ToList();
                // 添加发件组的发件箱
                var groupBoxes = await scopeServices.SqlContext.Outboxes.AsNoTracking()
                    .Where(x => outboxGroupIds.Contains(x.EmailGroupId)).ToListAsync();
                await AddSharedOutboxToPool(scopeServices, groupBoxes);
            }

            this._usableProxies = await PullUsableUserProxies(scopeServices);
            // 获取所有的模板
            this._allTemplates = await PullEmailTemplates(scopeServices);
            return true;
        }

        // 将新的发件箱添加到发件池中,自动去重了
        private async Task AddSharedOutboxToPool(SendingContext scopeServices, List<Outbox> outboxes)
        {
            if (outboxes.Count == 0) return;
            var outboxesPoolManager = scopeServices.ServiceProvider.GetRequiredService<UserOutboxesPoolManager>();

            var outboxAddresses = outboxes.ConvertAll(x => new OutboxEmailAddress(x, SendingGroupId, SmtpPasswordSecretKeys, OutboxEmailAddressType.Shared));
            foreach (var outbox in outboxAddresses)
            {
                await outboxesPoolManager.AddOutbox(scopeServices, outbox);
            }
        }


        /// <summary>
        /// 开始初始化
        /// 有可能会被多次调用
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitSendingItems(SendingContext sendingContext, List<long>? sendingItemIds)
        {
            // 获取待发件
            var dbSet = sendingContext.SqlContext.SendingItems.AsNoTracking()
                .Where(x => x.SendingGroupId == SendingGroupId)
                .Where(x => x.Status == SendingItemStatus.Failed || x.Status == SendingItemStatus.Created);
            if (sendingItemIds != null && sendingItemIds.Count > 0)
            {
                dbSet = dbSet.Where(x => sendingItemIds.Contains(x.Id));
            }
            List<SendingItem> toSendingItems = await dbSet.Select(x => new SendingItem() { Id = x.Id, OutBoxId = x.OutBoxId }).ToListAsync();
            HashSet<long> outboxIds = toSendingItems.Select(x => x.OutBoxId).ToHashSet();
            var outboxes = await sendingContext.SqlContext.Outboxes.Where(x => outboxIds.Contains(x.Id)).ToListAsync();

            // 对于找不到发件项的邮件，给予标记非法
            var invalidSendingItemIds = toSendingItems.Where(x => x.OutBoxId > 0).Where(x =>
            {
                var outbox = outboxes.FirstOrDefault(o => o.Id == x.OutBoxId);
                return outbox == null;
            }).Select(x => x.Id).ToList();
            if (invalidSendingItemIds.Count > 0)
            {
                // 更新邮件状态
                await sendingContext.SqlContext.SendingItems.UpdateAsync(x => invalidSendingItemIds.Contains(x.Id),
                    x => x.SetProperty(y => y.Status, SendingItemStatus.Invalid)
                        .SetProperty(y => y.SendDate, DateTime.Now)
                        .SetProperty(y => y.SendResult, "指定的发件箱已被删除"));
                toSendingItems = toSendingItems.FindAll(x => !invalidSendingItemIds.Contains(x.Id)).ToList();
            }

            // 新增特定发件箱
            var outboxesPoolManager = sendingContext.ServiceProvider.GetRequiredService<UserOutboxesPoolManager>();
            foreach (var outbox in outboxes)
            {
                // 获取收件项 Id
                var sendingItemIdsTemp = toSendingItems.Where(x => x.OutBoxId == outbox.Id).Select(x => x.Id).ToList();
                var outboxAddress = new OutboxEmailAddress(outbox, SendingGroupId, SmtpPasswordSecretKeys, OutboxEmailAddressType.Specific, sendingItemIdsTemp);
                await outboxesPoolManager.AddOutbox(sendingContext, outboxAddress);
            }

            // 更新待发件列表
            // 由于初始化时，不是并发的，不需要加锁
            var toSendingItemMetas = toSendingItems.Where(x => x.OutBoxId == 0).Select(x => new SendItemMeta(x))
                .Except(_sendingItemMetas)
                .ToList();
            toSendingItemMetas.ForEach(x =>
            {
                _sendingItemMetas.Add(x);
            });

            // 更新当前发件组的总数
            _sendingItemsCounter.IncreaseTotalCount(toSendingItemMetas.Count);

            // 将发件项修改为发送中
            if (toSendingItems.Count > 0)
            {
                await sendingContext.SqlContext.SendingItems.UpdateAsync(x => toSendingItems.Select(x => x.Id).Contains(x.Id),
                    x => x.SetProperty(y => y.Status, SendingItemStatus.Pending));
            }
            else
            {
                await sendingContext.SqlContext.SendingItems.UpdateAsync(x => x.SendingGroupId == SendingGroupId,
                                       x => x.SetProperty(y => y.Status, SendingItemStatus.Pending));
            }

            // 通知用户，任务已开始
            var hub = sendingContext.HubClient;
            var client = hub.GetUserClient(UserId);
            if (client != null)
            {
                await client.SendingGroupProgressChanged(new SendingGroupProgressArg(_sendingGroup, _startDate)
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
        private async Task<List<EmailTemplate>> PullEmailTemplates(SendingContext sendingContext)
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

            var templates = await sendingContext.SqlContext.EmailTemplates
                .AsNoTracking()
                .Where(x => templateIds.Contains(x.Id) && x.UserId == UserId)
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
        private async Task<List<OrganizationProxy>> PullUsableUserProxies(SendingContext scopeServices)
        {
            var user = await scopeServices.SqlContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == UserId);
            var results = await scopeServices.SqlContext.OrganizationProxies.Where(x => x.OrganizationId == user.OrganizationId)
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
        private string GetSendingItemOriginBody(SendItem sendItem)
        {
            var templates = _allTemplates;
            if (sendItem.IsSendingBatch)
            {
                // 说明是批量发送
                if (!string.IsNullOrEmpty(_sendingGroup.Body)) return _sendingGroup.Body;

                // 若本身有模板，则返回自己的模板
                if (sendItem.SendingItem.EmailTemplateId > 0)
                {
                    return templates.FirstOrDefault(x => x.Id == _sendingGroup.Id)?.Content ?? string.Empty;
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
            if (!string.IsNullOrEmpty(_sendingGroup.Body)) return _sendingGroup.Body;

            // 返回随机模板
            var sharedTemplates = templates.Where(x => x.Type.HasFlag(TemplateType.Shared)).ToList();
            if (sharedTemplates.Count == 0) return string.Empty;

            return sharedTemplates[new Random().Next(sharedTemplates.Count)].Content;
        }

        /// <summary>
        /// 该接口线程安全
        /// 从数据库获取 SendingItem，然后转换成 SendItem
        /// TODO: 可以将按批次将数据缓存
        /// </summary>
        /// <param name="outbox"></param>
        /// <returns></returns>
        private async Task<SendItem?> TakeSendItemFromSql(SendingContext scopeServices, OutboxEmailAddress outbox)
        {
            // 先发指定项
            SendItemMeta sendItemMeta = new(0);
            if (outbox.Type.HasFlag(OutboxEmailAddressType.Specific))
            {
                lock (outbox.SendingItemIdsLock)
                {
                    if (!outbox.SendingItemIds.IsEmpty)
                    {
                        outbox.SendingItemIds.TryDequeue(out var sendingItemId);
                        if (sendingItemId > 0)
                        {
                            sendItemMeta = new(sendingItemId);
                        }
                    }
                }
            }
            else
            {
                // 从当前组中获取
                if (!_sendingItemMetas.TryTake(out var exitSendItemMeta)) return null;
                sendItemMeta = exitSendItemMeta;
            }
            if (sendItemMeta.SendingItemId == 0) return null;
            // 标记正在运行
            _sendingItemsCounter.IncreaseRunningCount(1);

            var sendingItem = await scopeServices.SqlContext.SendingItems
                .AsNoTracking()
                .Where(x => x.Id == sendItemMeta.SendingItemId)
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync();
            // 一般不会出现空
            if (sendingItem == null)
            {
                _sendingItemsCounter.IncreaseSentCount(false);
                return null;
            }

            // 转换成 sendItem
            var sendItem = new SendItem(sendItemMeta, sendingItem)
            {
                // 添加数据
                BodyData = GetExcelData(sendingItem),
                HtmlBody = string.Empty,
            };

            // 生成正文
            sendItem.HtmlBody = GetSendingItemOriginBody(sendItem);

            // 生成主题
            // 若本身有主题，则使用自身的主题
            if (sendItem.BodyData != null && !string.IsNullOrEmpty(sendItem.BodyData.Subject))
            {
                sendItem.Subject = sendItem.BodyData.Subject;
            }
            else sendItem.Subject = _sendingGroup.GetRandSubject();
            return sendItem;
        }

        /// <summary>
        /// 获取发件项
        /// </summary>
        /// <returns></returns>
        public async Task<SendItem?> GetSendItem(SendingContext sendingContext, OutboxEmailAddress outboxEmailAddress)
        {
            // 判断是否为当前组对应的发件箱
            if (!outboxEmailAddress.SendingGroupIds.Contains(SendingGroupId)) return null;

            // 从列表中移除发件项并转换成 sendItem
            var sendItem = await TakeSendItemFromSql(sendingContext, outboxEmailAddress);
            if (sendItem == null) return null;
            sendingContext.SendItem = sendItem;

            // 判断收件箱是否牌冷却中
            // 从数据库中判断
            var (isCooling, message) = await IsInboxICooling(sendingContext.SqlContext, sendItem.Inboxes.ConvertAll(x => x.Id));
            if (isCooling)
            {
                // 更新邮件状态
                await sendingContext.SqlContext.SendingItems.UpdateAsync(x => x.Id == sendItem.SendingItem.Id,
                    x => x.SetProperty(y => y.Status, SendingItemStatus.Failed)
                        .SetProperty(y => y.SendDate, DateTime.Now)
                        .SetProperty(y => y.SendResult, message)
                );
                sendingContext.SetSendResult(new SendResult(false, message)
                {
                    SentStatus = SentStatus.Failed
                });
                return null;
            }

            // 为 sendItem 动态赋值
            // 赋予发件箱
            sendItem.Outbox = outboxEmailAddress;
            // 赋予回信人
            sendItem.ReplyToEmails = outboxEmailAddress.ReplyToEmails;
            var userSettingReader = await SettingsCache.GetSettingsReader(sendingContext.SqlContext, UserId);
            if (sendItem.ReplyToEmails.Count == 0)
            {
                // 使用全局回复               
                sendItem.ReplyToEmails = userSettingReader.ReplyToEmailsList;
            }

            // 添加重试次数
            sendItem.MaxRetryCount = userSettingReader.MaxRetryCount.Value;

            // 添加代理
            // 代理与发件箱进行匹配            
            sendItem.ProxyInfo = GetMatchedProxy(outboxEmailAddress, sendItem.SendingItem.ProxyId);

            // 推送开始发件
            var client = sendingContext.HubClient.GetUserClient(UserId);
            client?.SendingItemStatusChanged(new SendingItemStatusChangedArg(sendItem.SendingItem)
            {
                Status = SendingItemStatus.Sending
            });

            // 保存当前发件组任务
            sendingContext.SendingGroupTask = this;
            return sendItem;
        }

        /// <summary>
        /// 判断是否在冷却中
        /// </summary>
        /// <param name="inboxIds"></param>
        /// <returns></returns>
        private async Task<Tuple<bool, string>> IsInboxICooling(SqlContext sqlContext, List<long> inboxIds)
        {
            // 获取收件箱信息
            var inboxes = await sqlContext.Inboxes.Where(x => inboxIds.Contains(x.Id))
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
            var setting = await SettingsCache.GetSettingsReader(sqlContext, UserId);
            if (setting.MinInboxCooldownHours.Value <= 0) return Tuple.Create(false, "");

            var inboxesWhithGlobalCooling = inboxes.Where(x => x.MinInboxCooldownHours < 0).ToList();
            coolingInbox = inboxesWhithGlobalCooling.FirstOrDefault(x => x.LastSuccessDeliveryDate.AddHours(setting.MinInboxCooldownHours.Value) > DateTime.Now);
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

            // 随机获取匹配的代理
            var random = new Random();
            return _usableProxies.OrderBy(x => random.Next()).FirstOrDefault(x => x.IsMatch(outbox.Email))?.ToProxyInfo();
        }

        /// <summary>
        /// 邮件发送完成
        /// 只储存 邮件组 数据，具体的每次发件数据在 SendItem 中处理
        /// 只有失败或者成功才会触发，重发时不会触发
        /// </summary>
        /// <param name="sendingContext">发送结果</param>
        public async Task EmailItemSendCompleted(SendingContext sendingContext)
        {
            // 若是 outbox 连接错误，则移除所有相关发件项
            if (sendingContext.SendResult.SentStatus.HasFlag(SentStatus.OutboxError))
            {
                await RemoveSpecificSendingItems(sendingContext);
            }

            // 移除运行计数
            _sendingItemsCounter.IncreaseRunningCount(-1);

            var sendCompleteResult = sendingContext.SendResult;
            var sendItem = sendingContext.SendItem;

            // 若需要重试，则重新添加到队列中
            if (sendCompleteResult.SentStatus.HasFlag(SentStatus.Retry))
            {
                _sendingItemMetas.Add(sendingContext.SendItem.SendItemMeta);
            }
            else
            {
                // 只有已经发送成功或者不重试了，才更新发送进度
                _sendingItemsCounter.IncreaseSentCount(sendCompleteResult.Ok);
                var sqlContext = sendingContext.SqlContext;

                // 向数据库中保存状态
                var newSendingGroup = await sqlContext.SendingGroups.FirstAsync(x => x.Id == _sendingGroup.Id);
                newSendingGroup.SuccessCount = _sendingItemsCounter.TotalSuccessCount;
                newSendingGroup.SentCount = _sendingItemsCounter.TotalSentCount;
                newSendingGroup.LastMessage = sendCompleteResult.Message;
                await sqlContext.SaveChangesAsync();

                // 向用户推送发送组的进度            
                var client = sendingContext.HubClient.GetUserClient(UserId);
                if (client != null)
                {
                    // 推送发送组进度
                    await client.SendingGroupProgressChanged(new SendingGroupProgressArg(newSendingGroup, _startDate));

                    // 若已经完成，推送完成的进度
                    if (ShouldDispose)
                    {
                        await client.SendingGroupProgressChanged(new SendingGroupProgressArg(newSendingGroup, _startDate)
                        {
                            ProgressType = ProgressType.End
                        });
                    }
                }
            }

            // 调用外部回调
            await sendingContext.UserSendingGroupsPool.EmailItemSendCompleted(sendingContext);
        }

        /// <summary>
        /// 移除指定发件箱的发件项
        /// </summary>
        /// <param name="outboxEmail"></param>
        /// <returns></returns>
        public async Task RemoveSpecificSendingItems(SendingContext sendingContext)
        {
            var outboxEmail = sendingContext.OutboxEmailAddress;
            if (outboxEmail.SendingItemIds.IsEmpty) return;

            _sendingItemsCounter.IncreaseTotalCount(-outboxEmail.SendingItemIds.Count);
            // 对这些发件项标记失败
            await sendingContext.SqlContext.SendingItems.UpdateAsync(x => outboxEmail.SendingItemIds.Contains(x.Id),
                x => x.SetProperty(y => y.Status, SendingItemStatus.Failed)
                .SetProperty(y => y.SendDate, DateTime.Now)
                .SetProperty(y => y.SendResult, sendingContext.SendResult.Message));
        }

        public async Task NotifyEnd(SendingContext sendingContext, long sendingGroupId)
        {
            var client = sendingContext.HubClient.GetUserClient(UserId);
            if (client != null)
            {
                await client.SendingGroupProgressChanged(new SendingGroupProgressArg(sendingGroupId, _sendingItemsCounter, _startDate)
                {
                    ProgressType = ProgressType.End
                });
            }
        }
    }
}
