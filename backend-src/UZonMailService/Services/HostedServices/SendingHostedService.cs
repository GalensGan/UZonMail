using Quartz.Impl;
using Quartz;
using UZonMailService.Models.SqlLite;
using UZonMailService.Services.EmailSending;
using Microsoft.Extensions.Options;
using UZonMailService.Config;
using UZonMailService.Models.SqlLite.Init;
using Microsoft.EntityFrameworkCore;
using UZonMailService.Jobs;
using UZonMailService.Models.SqlLite.EmailSending;

namespace UZonMailService.Services.HostedServices
{
    /// <summary>
    /// 程序启动时，开始中断的发件任务
    /// </summary>
    public class SendingHostedService(IServiceScopeFactory ssf) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = ssf.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            await InitDatabase(serviceProvider);
            await InitScheduler(serviceProvider);
        }

        /// <summary>
        /// 初始化化数据库
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private static async Task InitDatabase(IServiceProvider serviceProvider)
        {
            var nv = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            var context = serviceProvider.GetRequiredService<SqlContext>();
            // 应用迁移
            context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            var appConfig = serviceProvider.GetRequiredService<IOptions<AppConfig>>();
            // 初始数据
            var initDb = new DatabaseInitializer(nv, context, appConfig.Value);
            initDb.Init();
        }

        /// <summary>
        /// 初始化调度器
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private static async Task InitScheduler(IServiceProvider serviceProvider)
        {
            var schdulerFactory = serviceProvider.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schdulerFactory.GetScheduler();

            #region 重置每日发件限制
            var jobKey = new JobKey($"schduleTask-resetSentCountToday");
            bool exist = await scheduler.CheckExists(jobKey);
            if (exist) return;

            var job = JobBuilder.Create<SentCountReseter>()
                .WithIdentity(jobKey)
                .Build();
            
            var trigger = TriggerBuilder.Create()
                .ForJob(jobKey)
                .StartAt(new DateTimeOffset(DateTime.Now.AddDays(1).Date)) // 明天凌晨开始
                .WithDailyTimeIntervalSchedule(x => x.WithIntervalInHours(24).OnEveryDay())
                .Build();
            await scheduler.ScheduleJob(job, trigger);
            #endregion
        }
    }
}
