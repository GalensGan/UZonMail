using UZonMailService.Services.EmailSending.Models;

namespace UZonMailService.Services.EmailSending.Event.Commands
{
    /// <summary>
    /// 泛型事件参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericCommand<T> : CommandBase
    {
        public GenericCommand(CommandType eventType, ScopeServices scopeServices, T? data) : base(eventType, scopeServices)
        {
            Data = data;
        }

        /// <summary>
        /// 附带的数据
        /// </summary>
        public T? Data { get; }
    }
}
