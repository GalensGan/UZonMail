using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using UZonMailDesktop.Models;
using UZonMailDesktop.Utils;

namespace UZonMailDesktop
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel { get; }
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();
            this.DataContext = ViewModel;

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            MainWebview2.CoreWebView2InitializationCompleted += MainWebview2_CoreWebView2InitializationCompleted;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new BackService().OnWindowsClosing();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var env = CoreWebView2Environment.CreateAsync(null, null, new CoreWebView2EnvironmentOptions()
            //{
            //    AdditionalBrowserArguments = "--disable-features=msEdgeMouseGestureSupported,msEdgeMouseGestureDefaultEnabled --enable-features=kEdgeMouseGestureDisabledInCN",

            //});
            //MainWebview2.EnsureCoreWebView2Async(env.Result);

            // 验证环境
            var envs = new List<IDetectRuntimeEnv>
            {
                new DotnetCoreEnvDetection(),
                new Webview2EnvDetection()
            };
            for (int i = 0; i < envs.Count; i++)
            {
                var env = envs[i];
                if (!env.DetectEnv())
                {
                    MessageBox.Show(env.FailedMessage, "环境缺失", MessageBoxButton.OK, MessageBoxImage.Error);
                    Process.Start(env.RedirectUrl);
                    Application.Current.Shutdown();
                    return;
                }
            }

            // 启动后端服务
            new BackService().StartBackService();
        }

        private void MainWebview2_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            var core = MainWebview2.CoreWebView2;
            core.Settings.AreDefaultContextMenusEnabled = true;
            core.Settings.IsScriptEnabled = true;

            //core.SetVirtualHostNameToFolderMapping("desktop.uzonmail.com", "wwwroot", CoreWebView2HostResourceAccessKind.DenyCors);
            //core.NavigationStarting += Core_NavigationStarting;            
        }

        private void Core_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // 只针对指定的 URL 进行拦截
            if (!e.Uri.Contains("desktop.uzonmail.com")) return;
            if (e.Uri.Equals(ViewModel.URL)) return;
            if (e.Uri.EndsWith(".html")) return;
            e.Cancel = true;
        }
    }
}
