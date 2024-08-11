using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.Utils.Plugin
{
    /// <summary>
    /// 插件
    /// </summary>
    public interface IPlugin
    {
        void UseServices(WebApplicationBuilder webApplicationBuilder);
        void UseApp(WebApplication webApplication);
    }
}
