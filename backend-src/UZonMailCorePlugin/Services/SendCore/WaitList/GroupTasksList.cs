﻿using log4net;
using System.Collections.Concurrent;
using UZonMail.Core.Services.SendCore.Contexts;
using UZonMail.Core.Services.SendCore.Interfaces;
using UZonMail.Core.Services.SendCore.Outboxes;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.Utils.Web.Service;

namespace UZonMail.Core.Services.SendCore.WaitList
{
    /// <summary>
    /// 系统级的待发件调度器
    /// </summary>
    public class GroupTasksList() : ISingletonService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(GroupTasksList));
        private readonly ConcurrentDictionary<long, GroupTasks> _userTasks = new();

        /// <summary>
        /// 将发件组添加到待发件队列
        /// 内部会自动向前端发送消息通知
        /// 内部已调用 CreateSendingGroup
        /// </summary>
        /// <param name="sendingContext">数据库上文</param>
        /// <param name="group"></param>
        /// <param name="sendingItemIds"></param>
        /// <returns></returns>
        public async Task<bool> AddSendingGroup(SendingContext sendingContext, SendingGroup group, List<long>? sendingItemIds = null)
        {
            if (group == null)
                return false;
            if (group.SmtpPasswordSecretKeys == null || group.SmtpPasswordSecretKeys.Count != 2)
            {
                _logger.Warn($"发送 {group.Id} 时, 没有提供 smtp 解密密钥，取消发送");
                return false;
            }

            // 判断是否有用户发件管理器
            if (!_userTasks.TryGetValue(group.UserId, out var groupTasks))
            {
                // 新建用户发件管理器
                groupTasks = new GroupTasks(group.UserId);
                _userTasks.TryAdd(group.UserId, groupTasks);
            }

            // 向发件管理器添加发件组
            bool result = await groupTasks.AddSendingGroup(sendingContext, group.Id, group.SmtpPasswordSecretKeys, sendingItemIds);
            return result;
        }

        /// <summary>
        /// 发送模块调用该方法获取发件项
        /// 若返回空，会导致发送任务暂停
        /// </summary>
        /// <returns></returns>
        public async Task<SendItemMeta?> GetEmailItem(SendingContext sendingContext)
        {
            var outbox = sendingContext.OutboxAddress;
            if (outbox == null)
            {
                _logger.Error("GetSendItem 调用失败, 请先获取发件箱");
                return null;
            }

            // 用户发件池
            var groupTasks = GetGroupTasks(outbox.UserId);
            if (groupTasks == null)
            {               
                return null;
            }

            return await groupTasks.GetEmailItem(sendingContext);
        }

        /// <summary>
        /// 已经对空的发件池进行了处理
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private GroupTasks? GetGroupTasks(long userId)
        {
            if (_userTasks.Count == 0)
            {
                _logger.Info("系统发件任务池为空");
                return null;
            }

            // 依次获取发件项
            // 返回 null 有以下几种情况：
            // 1. manager 为空
            // 2. 所有发件箱都在冷却中
            if (!_userTasks.TryGetValue(userId, out var sendingGroupsPool))
            {
                _logger.Info($"无法获取用户 {userId} 发件任务队列，该队列已释放");
                return null;
            }

            // 为空时移除
            if (sendingGroupsPool.Count == 0)
            {
                // 移除自己
                _userTasks.TryRemove(userId, out _);
                return null;
            }

            return sendingGroupsPool;
        }

        /// <summary>
        /// 邮件项发送完成回调
        /// </summary>
        /// <param name="sendingContext"></param>
        /// <returns></returns>
        public async Task EmailItemSendCompleted(SendingContext sendingContext)
        {
            // 清除数据
            // 移除用户队列池
            if (sendingContext.UserSendingGroupsPool.Count == 0)
            {
                _logger.Info($"用户 {sendingContext.UserSendingGroupsPool.UserId} 发件池为空，从发件池管理器中移除");
                _userTasks.TryRemove(sendingContext.UserSendingGroupsPool.UserId, out _);
            }

            // 回调发件箱处理
            await sendingContext.OutboxEmailAddress.EmailItemSendCompleted(sendingContext);
        }

        public async Task RemoveSendingGroupTask(long userId, long sendingGroupId)
        {
            if (!_userTasks.TryGetValue(userId, out var userSendingGroupsPool)) return;
            userSendingGroupsPool.TryRemove(sendingGroupId, out _);
        }
    }
}
