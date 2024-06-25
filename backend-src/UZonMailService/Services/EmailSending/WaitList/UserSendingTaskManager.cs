using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.EmailSending;
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
    /// </summary>
    public class UserSendingTaskManager : List<SendGroupTask>
    {
        private readonly SystemSendingWaitListService waitingList;
        private readonly IHubContext<UzonMailHub, IUzonMailClient> hub;
        private readonly UserOutboxesPool outboxesPool;
        private readonly SqlContext db;
        private readonly ILogger logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="waitingList"></param>
        /// <param name="hub"></param>
        /// <param name="outboxesPool"></param>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        public UserSendingTaskManager(
            AsyncServiceScope scope,
            SystemSendingWaitListService waitingList
            , IHubContext<UzonMailHub, IUzonMailClient> hub
            , UserOutboxesPool outboxesPool
            , SqlContext db,
            ILogger logger
            )
        {
            Scope = scope;
            this.waitingList = waitingList;
            this.hub = hub;
            this.outboxesPool = outboxesPool;
            this.db = db;
            this.logger = logger;

            outboxesPool.OutboxDisposed += OutboxesPool_OutboxDisposed;
        }

        // 发件箱因最大发件数限制被释放
        private void OutboxesPool_OutboxDisposed(OutboxEmailAddress emailAddress)
        {
            var tasksTemp = this.ToList();
            List<long> groupIds = tasksTemp.Select(x => x.GroupId).ToList();
            var intersectIds = groupIds.Intersect(emailAddress.SendingGroupIds).ToList();
            if (intersectIds.Count == 0) return;

            // 判断任务是否不存在发件箱，若不存在，则移除
            foreach (var task in tasksTemp)
            {
                int usableCount = outboxesPool.GetOutboxesCount(task.GroupId);
                if (usableCount != 0) continue;

                // 取消任务
                task.MarkCancelled("发件箱用尽", SendingGroupStatus.Finish).Wait();
                this.Remove(task);
            }
        }

        /// <summary>
        /// 用户 id
        /// </summary>
        public long UserId { get; set; }

        public AsyncServiceScope Scope { get; private set; }

        /// <summary>
        /// 添加发件组任务
        /// </summary>
        /// <param name="sendingGroupId"></param>
        /// <returns></returns>
        public async Task<bool> AddSendingGroup(long sendingGroupId, List<string> smtpPasswordSecretKeys, List<long>? sendingItemIds = null)
        {
            // 获取 sendingGroup
            var group = await db.SendingGroups.Where(x => x.Id == sendingGroupId)
                .Include(x => x.Templates)
                .Include(x => x.Outboxes)
                .FirstOrDefaultAsync();
            if (group == null) return false;
            if (group.UserId != UserId)
                return false;
            // 检查是否已经存在
            if (this.Any(t => t.GroupId == group.Id))
                return false;
            group.SmtpPasswordSecretKeys = smtpPasswordSecretKeys;

            // 有可能发件组已经存在
            var tasks = this.ToList();
            var existTask = tasks.FirstOrDefault(x => x.GroupId == group.Id);
            if (existTask == null)
            {
                // 重新初始化
                // 添加到列表
                var newTask = await SendGroupTask.Create(this, group, db, outboxesPool, hub, logger);
                var success = await newTask.InitSendingItems(sendingItemIds);
                if (!success) return false;
                this.Add(newTask);
            }
            else
            {
                // 复用原来的数据
                await existTask.InitSendingItems(sendingItemIds);
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
            var task = this.FirstOrDefault(t => t.GroupId == group.Id);
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
            var task = this.FirstOrDefault(t => t.GroupId == group.Id);
            if (task != null)
            {
                await task.MarkCancelled();

                // 移除并标记为释放
                this.Remove(task);
            }
        }

        /// <summary>
        /// 获取组中的发件项
        /// </summary>
        /// <returns></returns>
        public async Task<SendItem?> GetSendItem(SqlContext sqlContext)
        {
            if (outboxesPool.Count == 0)
                return null;

            if (this.Count == 0) return null;

            // 依次获取发件项
            SendItem? sendItem = null;
            for (int index = 0; index < this.Count; index++)
            {
                var groupTask = this[index];
                sendItem = await groupTask.GetSendItem(sqlContext);
                if (sendItem != null)
                {
                    break;
                }
            }

            return sendItem;
        }

        /// <summary>
        /// 邮件项发送完成
        /// </summary>
        /// <param name="sendCompleteResult"></param>
        /// <returns></returns>
        public async Task EmailItemSendCompleted(SendCompleteResult sendCompleteResult)
        {
            // 移除已经完成的任务
            // 获取需要移除的任务
            var tasks = this.Where(t => t.Status == SendingObjectStatus.ShouldDispose).ToList();
            // 移除发件池中的发件箱
            foreach (var task in tasks)
            {
                this.Remove(task);
                outboxesPool.RemoveOutbox(UserId, task.GroupId);
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
