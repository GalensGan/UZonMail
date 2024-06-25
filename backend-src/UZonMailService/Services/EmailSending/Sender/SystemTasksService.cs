using System.Timers;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SQL;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.WaitList;
using Timer = System.Timers.Timer;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 系统级发件调度中心
    /// </summary>
    public class SystemTasksService : ISingletonService
    {
        private IServiceScopeFactory ssf;
        private SystemSendingWaitListService waitList;
        private UserOutboxesPool outboxesPool;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="waitList"></param>
        /// <param name="outboxesPool"></param>
        public SystemTasksService(
              IServiceScopeFactory ssf
            , SystemSendingWaitListService waitList
            , UserOutboxesPool outboxesPool)
        {
            this.ssf = ssf;
            this.waitList = waitList;
            this.outboxesPool = outboxesPool;

            // 注册事件
            outboxesPool.OutboxCoolDownFinish += OutboxesPool_OutboxCoolDownFinish;
        }

        private void OutboxesPool_OutboxCoolDownFinish(int count)
        {
            // 开始发件
            StartSending(count);
        }

        private CancellationTokenSource? _tokenSource = new();
        /// <summary>
        /// 发件任务
        /// </summary>
        private readonly List<EmailSendingTask> _sendingTasks = [];

        #region 外部调用的方法
        /// <summary>
        /// 外部调用该方法开始发件
        /// 为了简化逻辑，每次调用会打开所有的线程
        /// </summary>
        public void StartSending(int activeCount = 0)
        {
            // 获取需要的发件数，根据发件数，智能增加任务
            int emailTypesCount = outboxesPool.GetOutboxesCount();
            // 获取核心数
            int coreCount = Environment.ProcessorCount;

            int tasksCount = Math.Min(emailTypesCount, coreCount);
            if (tasksCount == 0)
            {
                // 没有发件箱时，清理任务
                ClearTasks();
                return;
            }

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

        private Timer? _timer;
        /// <summary>
        /// 线程动态扩容
        /// </summary>
        private void DynamicExendTasks()
        {
            // 每 1 分钟检查一次
            _timer = new Timer(60000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }
        private void Timer_Elapsed(object? sender, ElapsedEventArgs? e)
        {
            // 每隔 1 分钟激活一次线程，防止线程一直处于等待状态
            StartSending();
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        private async Task DoWork(CancellationTokenSource tokenSource, AutoResetEventWrapper autoResetEvent)
        {
            // 当线程没有取消时
            while (!tokenSource.IsCancellationRequested)
            {
                // 激活后，从队列中取出任务
                var sendItem = await waitList.GetSendItem(autoResetEvent.SqlContext);
                if (sendItem == null)
                {
                    // 没有任务，继续等待
                    autoResetEvent.WaitOne();
                    continue;
                }

                // 发送邮件
                var sendMethod = sendItem.ToSendMethod();
                var status = await sendMethod.Send();
                if (status == SentStatus.Retry)
                {
                    // 发送失败，重新加入队列，可能会分配到其它线程去执行
                    sendItem.Enqueue();
                    // 通知收件
                    StartSending(1);
                    continue;
                }
            }
        }
        #endregion
    }
}
