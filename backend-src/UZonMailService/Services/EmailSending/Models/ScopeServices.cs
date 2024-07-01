using Microsoft.AspNetCore.SignalR;
using UZonMailService.Models.SQL;
using UZonMailService.SignalRHubs;

namespace UZonMailService.Services.EmailSending.Models
{
    /// <summary>
    /// 向发件任务传递的服务
    /// </summary>
    public class ScopeServices
    {
        public ScopeServices(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            SqlContext = serviceProvider.GetRequiredService<SqlContext>();
        }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public SqlContext SqlContext { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// 获取 SignalR 客户端
        /// </summary>
        public IHubContext<UzonMailHub, IUzonMailClient> HubClient => ServiceProvider.GetRequiredService<IHubContext<UzonMailHub, IUzonMailClient>>();
    }
}
