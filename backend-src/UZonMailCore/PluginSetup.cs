using Uamazing.Utils.Plugin;
using UZonMail.Core.Cache;
using UZonMail.Core.Config;
using UZonMail.Core.Services.HostedServices;
using UZonMail.Utils.Web.Service;
using UZonMail.Utils.Web;
using UZonMail.Core.SignalRHubs;
using UZonMail.Utils.Extensions;

namespace UZonMail.Core
{
    public class PluginSetup: IPlugin
    {
        public void UseApp(WebApplication webApplication)
        {
            // SignalR 配置
            webApplication.MapHub<UzonMailHub>($"/hubs/{nameof(UzonMailHub).ToCamelCase()}");
        }

        public void UseServices(WebApplicationBuilder webApplicationBuilder)
        {
            var services = webApplicationBuilder.Services;

            // 绑定配置
            services.Configure<AppConfig>(webApplicationBuilder.Configuration);
            // 添加数据缓存
            services.AddCache();

            // 批量注册服务
            services.AddServices();
            // 添加后台服务
            services.AddHostedService<SendingHostedService>();
        }
    }
}
