using EmbedIO;
using EmbedIO.Utilities;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Modules
{
    // 参考 https://github.com/unosquare/embedio/wiki/EmbedIO-and-IoC-containers#using-ioc-in-your-modules
    public class IoCModule : WebModuleBase
    {
        // Unique key for storing scopes in HTTP context items
        internal static readonly object ScopeKey = new object();

        private readonly IContainer _container;

        public IoCModule(IContainer container)
            : base(UrlPath.Root)
        {
            // We will need the container to create child scopes.
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        // Tell EmbedIO that this module does not handle requests completely.
        public override bool IsFinalHandler => false;

        protected override Task OnRequestAsync(IHttpContext context)
        {           
            // Store the scope for later retrieval.
            context.Items.Add(ScopeKey, _container);

            return Task.CompletedTask;
        }
    }
}
