using EmbedIO;
using Server.Http.Controller;
using Server.Http.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Extensions
{
    public static class HttpContextExtensions
    {
        // Shortcut method to retrieve an IoC scope from a context.
        public static StyletIoC.IContainer GetIoCScope(this IHttpContext @this) =>
             @this.Items[IoCModule.ScopeKey] as StyletIoC.IContainer ?? throw new ApplicationException("IoC scope not initialized for HTTP context");
    }
}
