﻿using log4net;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.EmailSending;
using UZonMailService.Services.EmailSending.Event;
using UZonMailService.Services.EmailSending.Event.Commands;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Pipeline;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.SignalRHubs;
using UZonMailService.SignalRHubs.Extensions;
using UZonMailService.SignalRHubs.SendEmail;

namespace UZonMailService.Services.EmailSending.WaitList
{
    /// <summary>
    /// 单个用户的发件任务管理
    /// 先添加先发送
    /// </summary>
    public class UserSendingGroupsPool : ConcurrentDictionary<long, SendingGroupTask>
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(UserSendingGroupsPool));

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userId"></param>
        public UserSendingGroupsPool(long userId)
        {
            UserId = userId;
        }

        #region 内部字段定义
        // 所有的发件箱
        private List<string> _outboxes = new();
        #endregion

        #region 公开属性

        #endregion

        /// <summary>
        /// 用户 id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 添加发件组任务
        /// 若包含 sendingItemIds，则只发送这部分邮件
        /// </summary>
        /// <param name="scopeServices"></param>
        /// <param name="sendingGroupId">传入时请保证组一定存在</param>
        /// <param name="smtpPasswordSecretKeys"></param>
        /// <param name="sendingItemIds"></param>
        /// <returns></returns>
        public async Task<bool> AddSendingGroup(SendingContext scopeServices, long sendingGroupId, List<string> smtpPasswordSecretKeys, List<long>? sendingItemIds = null)
        {
            // 有可能发件组已经存在
            if (!this.TryGetValue(sendingGroupId, out var existTask))
            {
                // 重新初始化
                // 添加到列表
                var newTask = await SendingGroupTask.Create(scopeServices, sendingGroupId, smtpPasswordSecretKeys);
                if (newTask == null) return false;

                var success = await newTask.InitSendingItems(scopeServices, sendingItemIds);
                if (!success) return false;
                this.TryAdd(sendingGroupId, newTask);
            }
            else
            {
                // 复用原来的数据
                await existTask.InitSendingItems(scopeServices, sendingItemIds);
            }

            return true;
        }

        /// <summary>
        /// 获取组中可被 outboxId 发送的邮件项
        /// </summary>
        /// <returns></returns>
        public async Task<SendItem?> GetSendItem(SendingContext scopeServices, OutboxEmailAddress outbox)
        {
            // 依次获取发件项
            foreach (var kv in this)
            {
                var groupTask = kv.Value;
                var sendItem = await groupTask.GetSendItem(scopeServices, outbox);
                if (sendItem != null)
                {
                    // 保存用户发件组池
                    scopeServices.UserSendingGroupsPool = this;
                    break;
                }
            }

            return null;
        }

        /// <summary>
        /// 邮件项发送完成
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        public async Task EmailItemSendCompleted(SendingContext sendingContext)
        {
            // 判断邮件任务是否已经发送完成
            if (!sendingContext.SendingGroupTask.ShouldDispose)
            {
                // 说明已经发完了
                // 移除当前任务
                this.TryRemove(sendingContext.SendingGroupTask.SendingGroupId,out _);
            }

            // 向上回调
            await sendingContext.UserSendingGroupsManager.EmailItemSendCompleted(sendingContext);
        }
    }
}
