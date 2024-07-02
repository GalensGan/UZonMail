using System.Collections.Concurrent;

namespace UZonMailService.Services.EmailSending.Base
{
    /// <summary>
    /// 用户队列
    /// </summary>
    public interface IDictionaryItem : IWeight
    {
        /// <summary>
        /// 是否处于可用状态
        /// </summary>
        bool Enable { get; }

        /// <summary>
        /// 键值
        /// </summary>
        string Key { get;}
    }
}
