﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Quartz;
using UZonMail.Core.Jobs;
using UZonMail.Core.Services.EmailSending.OutboxPool;
using UZonMail.Core.Services.EmailSending.Pipeline;
using UZonMail.Core.Services.EmailSending.Sender;
using UZonMail.Core.Services.EmailSending.WaitList;
using UZonMail.Core.Services.Settings;
using UZonMail.Core.Utils.Database;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.Utils.Web.Service;
using UZonMail.Utils.Extensions;
using UZonMail.Utils.Json;
using UZonMail.Core.Database.SQL.EmailSending;
using UZonMail.DB.SQL.Emails;
using UZonMail.Managers.Cache;
using UZonMail.DB.SQL.Settings;
using UZonMail.DB.SQL.Organization;

namespace UZonMail.Core.Services.EmailSending
{
    /// <summary>
    /// 发送组服务
    /// </summary>
    public class SendingGroupService(SqlContext db
        , TokenService tokenService
        , SendingThreadManager tasksService
        , UserSendingGroupsManager waitList
        , UserOutboxesPoolManager userOutboxesPoolManager
        , ISchedulerFactory schedulerFactory
        , IServiceProvider serviceProvider
        ) : IScopedService
    {
        /// <summary>
        /// 创建发送组
        /// </summary>
        /// <param name="sendingGroupData">该对象要求没有被ef跟踪</param>
        /// <returns></returns>
        public async Task<SendingGroup> CreateSendingGroup(SendingGroup sendingGroupData)
        {
            var userId = tokenService.GetUserDataId();

            // 格式化 Excel 数据
            sendingGroupData.Data = await FormatExcelData(sendingGroupData.Data, userId);
            // 使用事务
            await db.RunTransaction(async ctx =>
            {
                // 添加数据
                // 跟踪数据，将数据转换成 EF 对象
                if (sendingGroupData.Templates != null)
                {
                    var templates = ctx.EmailTemplates.Where(x => sendingGroupData.Templates.Select(t => t.Id).Contains(x.Id)).ToList();
                    sendingGroupData.Templates = templates;
                }
                if (sendingGroupData.Outboxes != null)
                {
                    var outboxes = ctx.Outboxes.Where(x => sendingGroupData.Outboxes.Select(t => t.Id).Contains(x.Id)).ToList();
                    sendingGroupData.Outboxes = outboxes;
                    sendingGroupData.OutboxesCount = outboxes.Count;
                }
                if (sendingGroupData.Attachments != null)
                {
                    var fileUsageIds = sendingGroupData.Attachments.Select(x => x.__fileUsageId).Where(x => x > 0).ToList();
                    if (fileUsageIds.Count > 0)
                    {
                        var attachmenets = await ctx.FileUsages.Where(x => fileUsageIds.Contains(x.Id)).ToListAsync();
                        sendingGroupData.Attachments = attachmenets;
                    }
                    else sendingGroupData.Attachments = [];
                }
                // 增加数据
                sendingGroupData.Status = SendingGroupStatus.Created;
                // 解析总数
                sendingGroupData.TotalCount = sendingGroupData.Inboxes.Count;
                sendingGroupData.UserId = userId;
                ctx.SendingGroups.Add(sendingGroupData);
                // 保存 group，从而获取 Id
                await ctx.SaveChangesAsync();

                // 获取用户设置
                var userReader = await CacheManager.GetCache<UserReader>(ctx, sendingGroupData.UserId.ToString());
                var settingsReader = await CacheManager.GetCache<OrganizationSettingReader>(ctx, userReader.OrganizationObjectId);

                // 保存发件箱
                await SaveInboxes(sendingGroupData.Data, sendingGroupData.UserId);

                // 将数据组装成 SendingItem 保存
                // 要确保数据已经通过验证
                var builder = new SendingItemsBuilder(db, sendingGroupData, settingsReader.MaxSendingBatchSize, tokenService);
                List<SendingItem> items = await builder.GenerateAndSave();

                // 更新发件总数量
                sendingGroupData.TotalCount = items.Count;
                // 更新发件箱的数量
                if (sendingGroupData.OutboxGroups != null && sendingGroupData.OutboxGroups.Count > 0)
                {
                    var outboxGroupIds = sendingGroupData.OutboxGroups.Select(x => x.Id).ToList();
                    var outboxCount = await db.Outboxes.AsNoTracking().Where(x => outboxGroupIds.Contains(x.EmailGroupId)).CountAsync();
                    sendingGroupData.OutboxesCount += outboxCount;
                }

                // 增加附件使用记录
                if (sendingGroupData.Attachments != null && sendingGroupData.Attachments.Count > 0)
                {
                    var attachmentObjectIds = sendingGroupData.Attachments.Select(x => x.FileObjectId).ToList();
                    await ctx.FileObjects.UpdateAsync(x => attachmentObjectIds.Contains(x.Id), obj => obj.SetProperty(x => x.LinkCount, y => y.LinkCount + 1));
                }

                return await ctx.SaveChangesAsync();
            });

            return sendingGroupData;
        }

        /// <summary>
        /// 格式化 Excel 数据
        /// 只保留属于自己的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<JArray?> FormatExcelData(JArray? data, long userId)
        {
            if (data == null || data.Count == 0)
            {
                return data;
            }

            // 获取发件箱,只能使用自己名下的发件箱
            var outboxEmails = data.Select(x => x.SelectTokenOrDefault("outbox", "")).Where(x => !string.IsNullOrEmpty(x)).ToList();
            var outboxes = await db.Outboxes.Where(x => x.UserId == userId && outboxEmails.Contains(x.Email)).ToListAsync();

            var templateIds = data.Select(x => x.SelectTokenOrDefault("templateId", 0L)).Where(x => x > 0).ToList();
            var templateNames = data.Select(x => x.SelectTokenOrDefault("templateName", "")).Where(x => !string.IsNullOrEmpty(x)).ToList();
            var templates = await db.EmailTemplates.Where(x => x.UserId == userId && (templateIds.Contains(x.Id) || templateNames.Contains(x.Name))).ToListAsync();

            // 重新更新数据
            JArray results = [];
            foreach (var token in data)
            {
                var outboxEmail = token.SelectTokenOrDefault<string>("outbox", "");
                if (!string.IsNullOrEmpty(outboxEmail))
                {
                    // 获取 outboxId
                    var outboxEntity = outboxes.FirstOrDefault(x => x.Email == outboxEmail);
                    if (outboxEntity != null)
                    {
                        token["outboxId"] = outboxEntity.Id;
                    }
                    else
                    {
                        token["outboxId"] = 0;
                        token["outbox"] = string.Empty;
                    }
                }

                var templateId = token.SelectTokenOrDefault<int>("templateId", 0);
                var templateName = token.SelectTokenOrDefault<string>("templateName", "");
                var templateEntity = templates.FirstOrDefault(x => x.Id == templateId || x.Name == templateName);

                if (templateEntity == null)
                {
                    token["templateId"] = 0;
                }
                else
                {
                    token["templateId"] = templateEntity.Id;
                }
                results.Add(token);
            }

            return results;
        }

        /// <summary>
        /// 保存数据中的收件箱
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task SaveInboxes(JArray? data, long userId)
        {
            if (data == null) return;
            var emails = data.Select(x => x["inbox"]).Where(x => x != null).Select(x => x.ToString()).ToList();
            if (emails.Count == 0) return;

            var existsEmails = await db.Inboxes.AsNoTracking()
                .IgnoreQueryFilters()
                .Where(x => x.UserId == userId && emails.Contains(x.Email))
                .Select(x => x.Email)
                .ToListAsync();

            var newEmails = emails.Except(existsEmails);
            // 新建 email
            // 查找默认的收件组
            var defaultInboxGroup = await db.EmailGroups.Where(x => x.Type == EmailGroupType.InBox && x.IsDefault).FirstOrDefaultAsync();
            if (defaultInboxGroup == null)
            {
                defaultInboxGroup = EmailGroup.GetDefaultEmailGroup(userId, EmailGroupType.InBox);
                db.EmailGroups.Add(defaultInboxGroup);
                await db.SaveChangesAsync();
            }

            // 某些发件箱可能被删除，恢复数据
            await db.Inboxes.IgnoreQueryFilters().Where(x => x.UserId == userId && x.IsDeleted && emails.Contains(x.Email))
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsDeleted, false));

            // 新建发件箱
            foreach (var email in newEmails)
            {
                var inbox = new Inbox()
                {
                    Email = email,
                    UserId = userId,
                    EmailGroupId = defaultInboxGroup.Id
                };
                inbox.SetStatusNormal();
                db.Inboxes.Add(inbox);
            }
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// 立即发件
        /// sendingGroup 需要提供 SmtpPasswordSecretKeys 参数
        /// </summary>
        /// <param name="sendingGroup"></param>
        /// <param name="sendItemIds">若有值，则只会发送这部分邮件</param>
        /// <returns></returns>
        public async Task SendNow(SendingGroup sendingGroup, List<long>? sendItemIds = null)
        {
            // 创建新的上下文
            var scopeServices = new SendingContext(serviceProvider);
            // 添加到发件列表
            await waitList.AddSendingGroup(scopeServices, sendingGroup, sendItemIds);
            // 开始发件
            tasksService.StartSending();
            // 更新状态
            await scopeServices.SqlContext.SendingGroups
                .UpdateAsync(x => x.Id == sendingGroup.Id, x => x.SetProperty(y => y.Status, SendingGroupStatus.Sending));
        }

        /// <summary>
        /// 计划发件
        /// </summary>
        /// <param name="sendingGroup"></param>
        /// <returns></returns>
        public async Task SendSchedule(SendingGroup sendingGroup)
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var jobKey = new JobKey($"emailSending-{sendingGroup.Id}", "sendingGroup");

            var job = JobBuilder.Create<EmailSendingJob>()
                        .WithIdentity(jobKey)
                        .SetJobData(new JobDataMap
                        {
                            { "sendingGroupId", sendingGroup.Id },
                            { "smtpPasswordSecretKeys", string.Join(',',sendingGroup.SmtpPasswordSecretKeys) }
                        })
                        .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(jobKey)
                .StartAt(new DateTimeOffset(sendingGroup.ScheduleDate))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// 移除发件任务
        /// 里面不会修改发件组和发件项的状态
        /// </summary>
        /// <returns></returns>
        public async Task RemoveSendingGroupTask(SendingGroup sendingGroup)
        {
            // 找到关联的发件箱移除
            await userOutboxesPoolManager.RemoveOutboxesBySendingGroup(sendingGroup.UserId, sendingGroup.Id);
            // 移除任务
            await waitList.RemoveSendingGroupTask(sendingGroup.UserId, sendingGroup.Id);
        }
    }
}
