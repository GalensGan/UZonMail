using EmbedIO;
using EmbedIO.Files;
using EmbedIO.WebApi;
using Server.Config;
using Swan.Logging;
using Server.Http.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using StyletIoC;
using Server.Http.Extensions;
using LiteDB;

namespace Server.Http
{
    class HttpServiceMain
    {
        private WebServer _server;
        private IContainer _container;
       
        public void Start(IContainer container)
        {
            _container = container;

            UserConfig userConfig = container.Get<UserConfig>();
            // 注册 swan 的日志库
            FileLogger logger = new FileLogger(userConfig.HttpLogPath, true);
            Logger.RegisterLogger(logger);

            var url = $"http://*:{UserConfig.Instance.HttpPort}/";
            // Our web server is disposable
            _server = CreateWebServer(url);

            // Once we've registered our modules and configured them, we call the RunAsync() method.
            _server.RunAsync();
        }

        // Create and configure our web server.
        private WebServer CreateWebServer(string url)
        {
            // 获取当前工作目录            
            string staticName = Path.Combine(UserConfig.Instance.RootDir, UserConfig.Instance.StaticName);
            Directory.CreateDirectory(staticName);

            // 配置
            WebServer server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                // 允许跨域
                .WithCors()
                // First, we will configure our web server by adding Modules.
                .WithLocalSessionManager();

            // 添加 Ioc
            server.WithIoC(_container);

            Logger.Info("开始加载 http 路由");
            // 添加路由
            // 获取所有继承于 baseController 的类
            Type[] types = Assembly.GetCallingAssembly().GetTypes();
            Type baseType = typeof(BaseController);
            List<Type> controllers = types.Where(t =>
            {
                return baseType == t.BaseType;
            }).ToList();
            string baseRout = UserConfig.Instance.BaseRoute;
            server.WithWebApi(baseRout, m => {
                controllers.ForEach(ctor => m.WithController(ctor));
            });
            Logger.Info("http 路由加载完成");


            Logger.Info("加载静态目录:" + staticName);
            server.WithStaticFolder("StaticFolder", "/", staticName, false, m =>
            {
                m.ContentCaching = true;
                m.Cache =new FileCache()
                {
                    MaxFileSizeKb = 204800,
                    MaxSizeKb = 1024000,
                };
                m.WithContentCaching(true);
            });
            Logger.Info("静态目录加载完成");

            // Listen for state changes.
            server.StateChanged += Server_StateChanged;
            return server;
        }

        private void Server_StateChanged(object sender, WebServerStateChangedEventArgs e)
        {
            $"WebServer New State - {e.NewState}".Info();
        }
    }
}
