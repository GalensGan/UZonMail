using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Models.SqlLite.UserInfos;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.Settings;
using UZonMailService.SignalRHubs;
using UZonMailService.Utils.Database;

namespace UZonMailService.Services.EmailSending.WaitList
{
    /// <summary>
    /// 系统级的待发件调度器
    /// 请求时，平均向各个用户请求资源
    /// 每位用户的资源都是公平的
    /// 今后可以考虑加入权重
    /// </summary>
    public class SystemSendingWaitListService(IServiceScopeFactory ssf) : ISingletonService
    {
        private readonly ConcurrentQueue<UserSendingTaskManager> _userTasks = new();

        /// <summary>
        /// 将发件组添加到待发件队列
        /// 内部会自动向前端发送消息通知
        /// 内部已调用 CreateSendingGroup
        /// </summary>
        /// <param name="group"></param>
        /// <param name="sendingItemIds"></param>
        /// <returns></returns>
        public async Task<bool> AddSendingGroup(SendingGroup group, List<int>? sendingItemIds = null)
        {
            if (group == null)
                return false;
            if (group.SmtpPasswordSecretKeys == null || group.SmtpPasswordSecretKeys.Count != 2)
                return false;

            // 判断是否有用户发件管理器
            var taskManager = _userTasks.FirstOrDefault(x => x.UserId == group.UserId);
            if (taskManager == null)
            {
                var scope = ssf.CreateAsyncScope();
                var sp = scope.ServiceProvider;
                var hub = sp.GetRequiredService<IHubContext<UzonMailHub, IUzonMailClient>>();
                var outboxesPool = sp.GetRequiredService<UserOutboxesPool>();
                var db = sp.GetRequiredService<SqlContext>();
                var logger = sp.GetRequiredService<ILogger<UserSendingTaskManager>>();
                // 新建用户发件管理器
                taskManager = new UserSendingTaskManager(scope, this, hub, outboxesPool, db, logger)
                {
                    UserId = group.UserId
                };
                _userTasks.Enqueue(taskManager);
            }

            // 向发件管理器添加发件组
            bool result = await taskManager.AddSendingGroup(group.Id, group.SmtpPasswordSecretKeys, sendingItemIds);

            return result;
        }

        /// <summary>
        /// 暂停发件
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool SwitchSendTaskStatus(SendingGroup group, bool pause)
        {
            if (group == null)
                return false;

            // 判断是否有用户发件管理器
            var taskManager = _userTasks.FirstOrDefault(x => x.UserId == group.UserId);
            if (taskManager == null)
                return false;

            taskManager.SwitchSendTaskStatus(group, pause);
            return true;
        }


        /// <summary>
        /// 取消发件
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task<bool> CancelSending(SendingGroup group)
        {
            if (group == null)
                return false;

            // 判断是否有用户发件管理器
            var taskManager = _userTasks.FirstOrDefault(x => x.UserId == group.UserId);
            if (taskManager == null)
                return false;

            await taskManager.CancelSending(group);
            return true;
        }

        /// <summary>
        /// 发送模块调用该方法获取发件项
        /// 若返回空，会导致发送任务暂停
        /// </summary>
        /// <returns></returns>
        public async Task<SendItem?> GetSendItem()
        {
            if (_userTasks.IsEmpty)
                return null;

            // 依次获取发件项
            // 返回 null 有以下几种情况：
            // 1. manager 为空
            // 2. 所有发件箱都在冷却中
            int queueCount = _userTasks.Count;
            int index = 0;
            SendItem? sendItem = null;
            while (index < queueCount && sendItem == null)
            {
                index++;

                if (!_userTasks.TryDequeue(out var manager)) continue;

                sendItem = await manager.GetSendItem();
                // 若为空，判断是否需要释放
                if (sendItem == null)
                {
                    var status = manager.GetManagerStatus();
                    if (status >= SendingObjectStatus.ShouldDispose)
                    {
                        // 不重新入队
                        // 释放资源
                        await manager.DisposeAsync();
                        // 释放
                        continue;
                    }
                }

                // 重新入队
                _userTasks.Enqueue(manager);
            }

            return sendItem;
        }

        /// <summary>
        /// 邮件项发送完成回调
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        public async Task EmailItemSendCompleted(bool success)
        {
            // 从队列中移除某项
            // 会在出队时处理，此处暂不处理
        }
    }
}
