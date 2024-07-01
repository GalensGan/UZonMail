using log4net;
using System.Timers;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SQL;
using UZonMailService.Services.EmailSending.Base;
using UZonMailService.Services.EmailSending.Event;
using UZonMailService.Services.EmailSending.Event.Commands;
using UZonMailService.Services.EmailSending.Models;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.WaitList;
using Timer = System.Timers.Timer;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 系统级发件中心
    /// </summary>
    public class SendingThreadManager : ISingletonService
    {
        private ILog _logger = LogManager.GetLogger(typeof(SendingThreadManager));

        private IServiceScopeFactory ssf;
        private UserSendingGroupsManager waitList;
        private UserOutboxesPoolManager outboxesPool;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="waitList"></param>
        /// <param name="outboxesPool"></param>
        public SendingThreadManager(
              IServiceScopeFactory ssf
            , UserSendingGroupsManager waitList
            , UserOutboxesPoolManager outboxesPool)
        {
            this.ssf = ssf;
            this.waitList = waitList;
            this.outboxesPool = outboxesPool;

            EventCenter.Core.DataChanged += EventCenter_DataChanged;

            // 自动激活，防止特殊情况被锁死
            SetAutoActive();
        }

        private async Task EventCenter_DataChanged(object? arg1, CommandBase arg2)
        {
            switch (arg2.CommandType)
            {
                case CommandType.StartSending:
                    {
                        if (arg2 is not StartSendingCommand startSendingCommand)
                            break;
                        StartSending(startSendingCommand.Data);
                        break;
                    }
                default:
                    break;
            }
        }

        private CancellationTokenSource? _tokenSource = new();
        /// <summary>
        /// 发件任务
        /// </summary>
        private readonly List<EmailSendingTask> _sendingTasks = [];

        #region 外部调用的方法
        /// <summary>
        /// 外部调用该方法开始发件
        /// 每次调用会打开指定数量的线程
        /// </summary>
        /// <param name="activeCount">若小于等于 0，则全部激活</param>
        public void StartSending(int activeCount = 0)
        {
            // 获取核心数
            int coreCount = Environment.ProcessorCount;
            // 保证数据库查询期间有其它任务处理任务
            int tasksCount = coreCount * 2;

            // 所有线程共用取消信号
            _tokenSource ??= new CancellationTokenSource();

            // 有可能任务已经存在，则只需要增量新建
            // 创建任务
            for (int i = _sendingTasks.Count; i < tasksCount; i++)
            {
                // 每个进程一个暂停信号
                var autoResetEvent = new AutoResetEventWrapper(false, ssf);
                EmailSendingTask task = new(async () =>
                {
                    // 任务开始
                    await DoWork(_tokenSource, autoResetEvent);
                }, _tokenSource)
                {
                    AutoResetEventWrapper = autoResetEvent
                };

                _sendingTasks.Add(task);
                task.Start();
            }

            if (activeCount <= 0)
            {
                // 全部激活
                // 激活特定数量的线程,使其工作
                for (int i = 0; i < tasksCount; i++)
                {
                    _sendingTasks[i].AutoResetEventWrapper.Set();
                }
            }
            else
            {
                // 只激活指定数量的线程
                for (int i = 0; i < tasksCount; i++)
                {
                    var task = _sendingTasks[i];
                    if (task.AutoResetEventWrapper.IsWaiting)
                    {
                        task.AutoResetEventWrapper.Set();
                        activeCount--;

                        // 激活达到指定数量后，退出
                        if (activeCount == 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private Timer? _timer;
        /// <summary>
        /// 每隔 1 分钟激活一次，防止因特殊原因锁死
        /// </summary>
        public void SetAutoActive()
        {
            if (_timer == null)
            {
                _timer = new Timer(60000)
                {
                    AutoReset = true
                };

                _timer.Elapsed += (sender, e) =>
                {
                    StartSending(1);
                };
                _timer.Start();
            }
        }

        /// <summary>
        /// 清除现有的任务
        /// </summary>
        private void ClearTasks()
        {
            if (_sendingTasks.Count == 0)
            {
                return;
            }

            // 取消所有任务
            _tokenSource?.Cancel();
            _tokenSource = null;
            _sendingTasks.Clear();
        }

        /// <summary>
        /// 开始任务
        /// 以发件箱的数据为索引进行发件，提高发件箱利用率
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        private async Task DoWork(CancellationTokenSource tokenSource, AutoResetEventWrapper autoResetEvent)
        {
            // 当线程没有取消时
            while (!tokenSource.IsCancellationRequested)
            {
                // 若没有数据上下文，报错
                if (autoResetEvent.Scope == null)
                {
                    _logger.Error("在线程中获取作用域失败");
                    autoResetEvent.WaitOne();
                    continue;
                }

                // 生成服务
                var scopeServices = new ScopeServices(autoResetEvent.Scope.ServiceProvider);

                // 取出发件箱
                // 并解析发件箱的代理信息
                var outboxResult = await outboxesPool.GetOutboxByWeight(scopeServices);
                if (outboxResult.NotOk)
                {
                    // 没有可用发件箱，继续等待
                    // 有可能处于冷却中
                    autoResetEvent.WaitOne();
                    continue;
                }

                var outbox = outboxResult.Data;
                // 取出该发件箱对应的邮件数据
                var sendItem = await waitList.GetSendItem(scopeServices, outbox);
                if (sendItem == null)
                {
                    // 没有任务，继续等待
                    autoResetEvent.WaitOne();
                    continue;
                }

                // 发送邮件
                var sendMethod = sendItem.ToSendMethod();
                var sendResult = await sendMethod.Send();
                // 若是发件箱错误，则将发件箱从队列中移除
                if (sendResult.SentStatus.HasFlag(SentStatus.OutboxConnectError) && sendResult.SendItem != null)
                {
                    outboxesPool.RemoveOutbox(sendResult.SendItem.SendingItem.UserId, sendResult.SendItem.Outbox.Email);
                }

                // 清理资源

                if (sendResult.SentStatus.HasFlag(SentStatus.Retry))
                {
                    // 发送失败，重新加入队列，可能会分配到其它线程去执行
                    sendItem.Enqueue();
                    // 通知收件
                    StartSending(1);
                }
            }
        }
        #endregion
    }
}
