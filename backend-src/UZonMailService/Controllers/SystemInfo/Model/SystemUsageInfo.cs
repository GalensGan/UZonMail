using System.Diagnostics;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.EmailSending.WaitList;

namespace UZonMailService.Controllers.SystemInfo.Model
{
    public class SystemUsageInfo
    {
        public double CpuUsage { get; private set; }

        public double MemoryUsage { get; private set; }

        public int RunningTasksCount { get; private set; }

        public List<OutboxPoolInfo> OutboxPoolInfos { get; set; }
        public List<SendingGroupInfo> SendingGroupsPoolInfos { get; set; }

        public async Task GatherInfomations(UserSendingGroupsManager userSendingGroupsManager,UserOutboxesPoolsManager userOutboxesPoolManager, SendingThreadManager sendingThreadManager)
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
