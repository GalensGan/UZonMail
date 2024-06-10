using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UzonMailDesktop
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            MainWebview2.CoreWebView2InitializationCompleted += MainWebview2_CoreWebView2InitializationCompleted;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseBackService();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var env = CoreWebView2Environment.CreateAsync(null, null, new CoreWebView2EnvironmentOptions()
            //{
            //    AdditionalBrowserArguments = "--disable-features=msEdgeMouseGestureSupported,msEdgeMouseGestureDefaultEnabled --enable-features=kEdgeMouseGestureDisabledInCN",

            //});
            //MainWebview2.EnsureCoreWebView2Async(env.Result);

            // 启动后端服务
            CloseBackService();

            var startInfo = new ProcessStartInfo
            {
                FileName = "UZonMailService.exe",
                WorkingDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "service"),
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(startInfo);
        }

        private void MainWebview2_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            var core = MainWebview2.CoreWebView2;
            core.Settings.AreDefaultContextMenusEnabled = true;
            core.Settings.IsScriptEnabled = true;
            core.SetVirtualHostNameToFolderMapping("desktop.uzonmail.com", "wwwroot", Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
        }

        /// <summary>
        /// 关闭后台服务
        /// </summary>
        private void CloseBackService()
        {
            // 查找进程名为 UZonMailService 的进程并杀死
            var process = Process.GetProcesses().Where(x => x.ProcessName == "UZonMailService").ToList();
            foreach (var item in process)
            {
                item.Kill();
            }
        }
    }
}
