using UZonMailService.Models.SQL;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 发送完成的结果
    /// </summary>
    public class SendResult
    {
        public SendResult(SendItem sendItem, bool ok, string message)
        {
            SendItem = sendItem;
            Ok = ok;
            Message = message;
        }


        /// <summary>
        /// 数据库上下文
        /// </summary>
        public SqlContext SqlContext => SendItem.SqlContext;

        /// <summary>
        /// 发送项
        /// 里面有当前进程的数据库上下文
        /// </summary>
        public SendItem SendItem { get;}

        /// <summary>
        /// 是否ok
        /// </summary>
        public bool Ok { get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get;}

        /// <summary>
        /// 发送状态
        /// </summary>
        public SentStatus SentStatus { get; set; } = SentStatus.OK;
    }
}
