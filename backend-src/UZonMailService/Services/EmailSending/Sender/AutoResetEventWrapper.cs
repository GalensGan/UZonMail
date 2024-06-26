using UZonMailService.Models.SQL;

namespace UZonMailService.Services.EmailSending.Sender
{
    public class AutoResetEventWrapper
    {
        private IServiceScopeFactory ssf;
        private AutoResetEvent _autoResetEvent;

        /// <summary>
        /// 是否处于等待状态
        /// </summary>
        public bool IsWaiting { get; private set; } = false;

        private IServiceScope _scope;
        public SqlContext SqlContext { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public AutoResetEventWrapper(bool initialState, IServiceScopeFactory ssf)
        {
            this.ssf = ssf;
            IsWaiting = initialState;
            _autoResetEvent = new AutoResetEvent(IsWaiting);

            // 创建一个数据上下文
            UpdateSqlContext();
        }

        /// <summary>
        /// 更新数据库上下文
        /// </summary>
        /// <returns></returns>
        private bool UpdateSqlContext()
        {
            if (ssf == null) return false;

            // 重新创建数据库上下文
            _scope = ssf.CreateAsyncScope();
            SqlContext = _scope.ServiceProvider.GetRequiredService<SqlContext>();
            return true;
        }

        /// <summary>
        /// 使线程继续
        /// </summary>
        public void Set()
        {
            IsWaiting = false;
            // 重新创建数据库上下文
            UpdateSqlContext();

            _autoResetEvent.Set();
        }

        /// <summary>
        /// 使线程等待
        /// </summary>
        private void Reset()
        {
            IsWaiting = true;
            _autoResetEvent.Reset();
        }

        /// <summary>
        /// 等待信号
        /// </summary>
        public void WaitOne()
        {
            IsWaiting = true;

            // 释放数据库连接
            _scope?.Dispose();

            _autoResetEvent.WaitOne();
        }
    }
}
