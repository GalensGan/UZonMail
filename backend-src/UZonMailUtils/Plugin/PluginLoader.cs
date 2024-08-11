using log4net;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Uamazing.Utils.Plugin
{
    /// <summary>
    /// 插件加载器
    /// </summary>
    public class PluginLoader : IPlugin
    {
        private ILog _logger = LogManager.GetLogger(typeof(PluginLoader));
        private readonly string _pluginDir;
        private List<string> _pluginDllFullPaths;

        private List<IPlugin> _plugins = [];

        public PluginLoader(string pluginDir)
        {
            _pluginDir = pluginDir;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            LoadPlugin();
        }

        private List<string>? _allDllNames;
        private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            _allDllNames ??= [.. Directory.GetFiles(_pluginDir, "*.dll", SearchOption.AllDirectories)];

            var dllName = Path.GetFileNameWithoutExtension(args.Name);
            var dllFullName = _allDllNames.Where(x => x.EndsWith(dllName + ".dll")).FirstOrDefault();

            return dllFullName == null ? null : Assembly.LoadFile(dllFullName);
        }

        /// <summary>
        /// 开始加载插件
        /// </summary>
        private void LoadPlugin()
        {
            // 获取所有插件的 dll 名称
            _pluginDllFullPaths = [.. Directory.GetFiles(_pluginDir, "*Plugin.dll", SearchOption.AllDirectories)];

            if (_pluginDllFullPaths.Count == 0)
            {
                return;
            }

            // 加载插件
            foreach (var dllFullPath in _pluginDllFullPaths)
            {
                // 判断是否已经加载了
                if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.Location == dllFullPath))
                {
                    continue;
                }

                var dll = Assembly.LoadFrom(dllFullPath);
                var thisType = typeof(PluginLoader);
                var pluginTypes = dll.GetTypes().Where(x => !x.IsAbstract && typeof(IPlugin).IsAssignableFrom(x) && x != thisType);

                foreach (var pluginType in pluginTypes)
                {
                    var pluginName = Path.GetFileNameWithoutExtension(dllFullPath);
                    if (Activator.CreateInstance(pluginType) is not IPlugin plugin)
                    {
                        _logger.Warn($"插件 {pluginName} 未实现 IPlugin 接口");
                        continue;
                    }

                    // 开始加载
                    _plugins.Add(plugin);
                    _logger.Info($"已加载插件: {pluginName}");
                }
            }
        }

        public void UseApp(WebApplication webApplication)
        {
            foreach (var item in _plugins)
            {
                item.UseApp(webApplication);
            }
        }

        public void UseServices(WebApplicationBuilder webApplicationBuilder)
        {
            foreach (var item in _plugins)
            {
                item.UseServices(webApplicationBuilder);
            }
        }
    }
}
