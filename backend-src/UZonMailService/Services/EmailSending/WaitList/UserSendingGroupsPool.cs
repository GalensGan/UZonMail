using log4net;
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
using UZonMailService.Services.EmailSending.Models;
using UZonMailService.Services.EmailSending.OutboxPool;
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
    public class UserSendingGroupsPool : ConcurrentBag<SendingGroupTask>
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(UserSendingGroupsPool));

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userId"></param>
        public UserSendingGroupsPool(long userId)
        {
            UserId = userId;
            // 注册发作箱释放事件
            EventCenter.Core.DataChanged += EventCenter_DataChanged;
        }

        #region 事件监听
        private async Task EventCenter_DataChanged(object? arg1, CommandBase arg2)
        {
            switch (arg2.CommandType)
            {
                case CommandType.OutboxDisposed:
                    OutboxesPool_OutboxDisposed(arg2);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// 发件箱被释放了
        /// </summary>
        /// <param name="args"></param>
        private void OutboxesPool_OutboxDisposed(CommandBase command)
        {
            if (outboxEmailAddress == null)
            {
                return;
            }

            // 移除发件项相关的发件项


            var tasksTemp = this.ToList();
            List<long> groupIds = tasksTemp.Select(x => x.SendingGroupId).ToList();
            var intersectIds = groupIds.Intersect(emailAddress.SendingGroupIds).ToList();
            if (intersectIds.Count == 0) return;

            // 判断任务是否不存在发件箱，若不存在，则移除
            foreach (var task in tasksTemp)
            {
                int usableCount = outboxesPool.GetOutboxesCount(task.SendingGroupId);
                if (usableCount != 0) continue;

                // 取消任务
                task.MarkCancelled("发件箱用尽", SendingGroupStatus.Finish).Wait();
                this.Remove(task);
            }
        }
        #endregion

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

        public AsyncServiceScope Scope { get; private set; }

        /// <summary>
        /// 添加发件组任务
        /// 若包含 sendingItemIds，则只发送这部分邮件
        /// </summary>
        /// <param name="scopeServices"></param>
        /// <param name="sendingGroupId">传入时请保证组一定存在</param>
        /// <param name="smtpPasswordSecretKeys"></param>
        /// <param name="sendingItemIds"></param>
        /// <returns></returns>
        public async Task<bool> AddSendingGroup(ScopeServices scopeServices, long sendingGroupId, List<string> smtpPasswordSecretKeys, List<long>? sendingItemIds = null)
        {
            // 有可能发件组已经存在
            var existTask = this.FirstOrDefault(x => x.SendingGroupId == sendingGroupId);
            if (existTask == null)
            {
                // 重新初始化
                // 添加到列表
                var newTask = await SendingGroupTask.Create(scopeServices, sendingGroupId, smtpPasswordSecretKeys);
                if (newTask == null) return false;

                var success = await newTask.InitSendingItems(scopeServices, sendingItemIds);
                if (!success) return false;
                this.Add(newTask);
            }
            else
            {
                // 复用原来的数据
                await existTask.InitSendingItems(scopeServices, sendingItemIds);
            }

            return true;
        }

        /// <summary>
        /// 暂停发件
        /// </summary>
        /// <param name="group"></param>
        /// <param name="pause"></param>
        /// <returns></returns>
        public void SwitchSendTaskStatus(SendingGroup group, bool pause)
        {
            // 查找发件组
            var task = this.FirstOrDefault(t => t.SendingGroupId == group.Id);
            if (task != null)
            {
                // 暂停发件
                task.Paused = pause;
            }
        }

        /// <summary>
        /// 取消发送
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task CancelSending(SendingGroup group)
        {
            var task = this.FirstOrDefault(t => t.SendingGroupId == group.Id);
            if (task != null)
            {
                await task.MarkCancelled();

                // 移除并标记为释放
                this.Remove(task);
            }
        }

        /// <summary>
        /// 获取组中可被 outboxId 发送的邮件项
        /// </summary>
        /// <returns></returns>
        public async Task<SendItem?> GetSendItem(SqlContext sqlContext, OutboxEmailAddress outbox)
        {
            // 依次获取发件项
            foreach (var groupTask in this)
            {
                var sendItem = await groupTask.GetSendItem(sqlContext, outbox);
                if (sendItem != null)
                {
                    break;
                }
            }

            return null;
        }

        /// <summary>
        /// 邮件项发送完成
        /// </summary>
        /// <param name="sendCompleteResult"></param>
        /// <returns></returns>
        public async Task EmailItemSendCompleted(SendResult sendCompleteResult)
        {
            // 移除已经完成的任务
            // 获取需要移除的任务
            var tasks = this.Where(t => t.Status == SendingObjectStatus.ShouldDispose).ToList();
            // 移除发件池中的发件箱
            foreach (var task in tasks)
            {
                this.Remove(task);
                outboxesPool.RemoveOutbox(UserId, task.SendingGroupId);
            }

            // 向上回调
            await waitingList.EmailItemSendCompleted(sendCompleteResult);
        }

        /// <summary>
        /// 获取管理器状态
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public SendingObjectStatus GetManagerStatus()
        {
            // 动态计算
            // 若发件池为空，则释放资源
            if (outboxesPool.GetExistOutboxes(UserId).Count == 0)
                return SendingObjectStatus.ShouldDispose;

            // 若所有的状态都是 shouldDispose 时，返回
            if (this.All(t => t.Status == SendingObjectStatus.ShouldDispose))
                return SendingObjectStatus.ShouldDispose;

            return SendingObjectStatus.Normal;
        }

        /// <summary>
        /// 异步释放
        /// </summary>
        /// <returns></returns>
        public async Task DisposeAsync()
        {
            await Scope.DisposeAsync();
            this.Clear();
        }
    }
}
