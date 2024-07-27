using System.Diagnostics;
using UZonMail.Core.Services.EmailSending.OutboxPool;
using UZonMail.Core.Services.EmailSending.Sender;
using UZonMail.Core.Services.EmailSending.WaitList;

namespace UZonMail.Core.Controllers.SystemInfo.Model
{
    public class SystemUsageInfo
    {
        public double CpuUsage { get; private set; }

        public double MemoryUsage { get; private set; }

        public int RunningTasksCount { get; private set; }

        public List<OutboxPoolInfo> OutboxPoolInfos { get; set; }
        public List<SendingGroupInfo> SendingGroupsPoolInfos { get; set; }

        public async Task GatherInfomations(UserSendingGroupsManager userSendingGroupsManager, UserOutboxesPoolManager userOutboxesPoolManager, SendingThreadManager sendingThreadManager)
        {
            CpuUsage = await GetCpuUsageForProcess();
            MemoryUsage = Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024;

            OutboxPoolInfos = userOutboxesPoolManager.GetOutboxPoolInfos();
            RunningTasksCount = sendingThreadManager.RunningTasksCount;
            SendingGroupsPoolInfos = userSendingGroupsManager.GetSendingGroupInfos();
        }

        private async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }
    }
}
