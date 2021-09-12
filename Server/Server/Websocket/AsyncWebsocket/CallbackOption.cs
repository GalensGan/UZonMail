using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Server.Websocket.AsyncWebsocket
{
    class CallbackOption<T>
    {
        public string Id { get; }        
        private Timer _timer;

        public double Interval { get; set; } = 10 * 1000;
        public TaskCompletionSource<T> TaskCompletion { get; private set; }

        public CallbackOption()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Task<T> Run(Action action)
        {
            TaskCompletion = new TaskCompletionSource<T>();
            _timer = new Timer(Interval);
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = false;
            _timer.Enabled = true;

            Task.Run(()=>
            {
                // 启动timer
                _timer.Start();

                // 执行任务
                action();
            });

            return TaskCompletion.Task;
        }

        /// <summary>
        /// 回调失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
           TaskCompletion.TrySetResult(default);
        }
    }
}
