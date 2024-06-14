using Quartz.Impl;
using Quartz;
using UZonMailService.Models.SqlLite;
using UZonMailService.Services.EmailSending;
using Microsoft.Extensions.Options;
using UZonMailService.Config;
using UZonMailService.Models.SqlLite.Init;
using Microsoft.EntityFrameworkCore;

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
        }

        private async Task InitDatabase(IServiceProvider serviceProvider)
        {
            var nv = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            var context = serviceProvider.GetRequiredService<SqlContext>();
            // 应用迁移
            context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            var appConfig = serviceProvider.GetRequiredService<IOptions<AppConfig>>();
            // 初始数据
            var initDb = new InitDatabase(nv, context, appConfig.Value);
            initDb.Init();
        }
    }
}
