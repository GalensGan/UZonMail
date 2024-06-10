
namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 发件测试器
    /// </summary>
    /// <param name="sendItem"></param>
    public class TestSender(SendItem sendItem) : SendMethod
    {
        public override async Task<SentStatus> Send()
        {
            // 开始发送，标记状态为正在发送
            

            // 等待 2s
           await Task.Delay(2000);

            // 结束发送，标记状态为发送成功

            // 返回结果
            return SentStatus.OK;
        }
    }
}
