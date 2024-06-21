using Quartz;
using Uamazing.Utils.Web.Service;

namespace UZonMailService.Jobs
{
    /// <summary>
    /// 发送计数重置任务
    /// </summary>
    public class SentCountReseter : IJob, IScopedService
    {
        public Task Execute(IJobExecutionContext context)
        {
            // 重置发送计数
            return Task.CompletedTask;
        }
    }
}
