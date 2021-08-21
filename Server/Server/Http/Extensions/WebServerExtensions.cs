using EmbedIO;
using Server.Http.Modules;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Extensions
{
    public static class WebServerExtensions
    {
        // Fluent extension method to add an IoC module to a web server.
        public static TServer WithIoC<TServer>(this TServer @this, IContainer container)
            where TServer : WebServer
        {
            @this.Modules.Add("IoCModule", new IoCModule(container));
            return @this;
        }
    }
}
