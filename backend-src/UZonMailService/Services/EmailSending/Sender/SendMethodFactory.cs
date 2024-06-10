namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 发件方法的工厂
    /// 若指定了发件方法，则返回指定的发件方法
    /// 否则按照规则进行本地与远程循环发送
    /// </summary>
    public class SendMethodFactory
    {
        /// <summary>
        /// 构建发件方法
        /// 目前纯按本机进行设计，后期可扩展为本地与远程循环发送
        /// </summary>
        /// <param name="sendItem"></param>
        /// <returns></returns>
        public static SendMethod BuildSendMethod(SendItem sendItem)
        {
            if (sendItem.SendItemType == SendItemType.Local)
                return new LocalSender(sendItem);
            
            // 其它情况
            return new LocalSender(sendItem);
        }
    }
}
