using System;
using Server.Http;
using Stylet;

namespace Server.Pages
{
    public class ShellViewModel : Screen
    {
        private HttpServiceMain _httpServer;
        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            // 加载静态网页服务
            _httpServer = new HttpServiceMain();
            _httpServer.Start();
        }
    }
}
