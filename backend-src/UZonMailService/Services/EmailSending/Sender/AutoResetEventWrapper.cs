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

        /// <summary>
        /// 作用域
        /// </summary>
        public IServiceScope? Scope { get; private set; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public SqlContext? SqlContext { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public AutoResetEventWrapper(bool initialState, IServiceScopeFactory ssf)
        {
            this.ssf = ssf;
            IsWaiting = initialState;
            _autoResetEvent = new AutoResetEvent(IsWaiting);

            // 创建一个 IoC 上下文
            UpdateOrCreateScope();
        }

        /// <summary>
        /// 更新数据库上下文
        /// </summary>
        /// <returns></returns>
        private bool UpdateOrCreateScope()
        {
            // 重新创建数据库上下文
            Scope = ssf.CreateAsyncScope();
            return true;
        }

        private void DisposeScope()
        {
            Scope?.Dispose();
            Scope = null;
        }

        /// <summary>
        /// 使线程继续
        /// </summary>
        public void Set()
        {
            IsWaiting = false;

            // 释放原来的数据库上下文
            DisposeScope();
            UpdateOrCreateScope();

            _autoResetEvent.Set();
        }

        /// <summary>
        /// 等待信号
        /// </summary>
        public void WaitOne()
        {
            IsWaiting = true;

            // 释放 IoC 上下文
            DisposeScope();

            _autoResetEvent.WaitOne();
        }
    }
}
