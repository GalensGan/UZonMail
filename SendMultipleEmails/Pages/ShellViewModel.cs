using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalensSDK.StyletEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats.Dml;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using SendMultipleEmails.Database;
using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
using SendMultipleEmails.ResponseJson;
using Stylet;

namespace SendMultipleEmails.Pages
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public class ShellViewModel : KeyOneActive<ScreenChild>
    {
        public void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem tvi = e.NewValue as TreeViewItem;

            base.InvokeTo(new InvokeParameter() { InvokeId = tvi.Name });
        }

        #region 属性

        public Store Store { get; private set; }

        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        #endregion

        public ShellViewModel(IWindowManager windowManager)
        {
            Store = new Store(windowManager);
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (Store.CurrentAccount.IsAutoSave)
            {
                Store.Save();
                return;
            }
            // 保存Store数据
            System.Windows.MessageBoxResult dialogResult = MessageBoxX.Show("是否保存数据？", "保存", null, MessageBoxButton.YesNoCancel,
                new MessageBoxXConfigurations() { CancelButton = "自动保存" });
            if (dialogResult == MessageBoxResult.Yes)
            {
                Store.Save();
            }
            else if (dialogResult == MessageBoxResult.Cancel)
            {
                // 自动保存
                Store.CurrentAccount.IsAutoSave = true;
                // 更新数据库，保存本地数据
                Store.GetAccountDatabase<IAccountDb>().UpdateAccount(Store.CurrentAccount);
            }
        }

        protected override void OnInitialActivate()
        {
            Screen loginVM = new LoginViewModel(Store);
            Store.WindowManager.ShowDialog(loginVM);

            #region 主要界面
            RegisterItem(new DashboardViewModel(Store)
            {
                DisplayName = "个人中心",
                ID = InvokeID.Dashboard.ToString(),
            });

            RegisterItem(new SendSettingsViewModel(Store)
            {
                DisplayName = "发送设置",
                ID = InvokeID.Settings.ToString(),
            });

            RegisterItem(new SendersViewModel(Store)
            {
                DisplayName = "发件人",
                ID = InvokeID.Senders.ToString(),
            });

            RegisterItem(new ReceiversViewModel(Store)
            {
                DisplayName = "收件人",
                ID = InvokeID.Receivers.ToString(),
            });

            RegisterItem(new SendDataViewModel(Store)
            {
                DisplayName = "导入数据",
                ID = InvokeID.ImportVariables.ToString(),
            });

            RegisterItem(new TemplateViewModel(Store)
            {
                DisplayName = "模板",
                ID = InvokeID.Template.ToString(),
            });

            RegisterItem(new SendViewModel(Store)
            {
                DisplayName = "发送",
                ID = InvokeID.Send.ToString(),
            });

            RegisterItem(new LogViewModel(Store)
            {
                DisplayName = "日志",
                ID = InvokeID.Log.ToString(),
            });

            RegisterItem(new AboutMeViewModel(Store)
            {
                DisplayName = "关于",
                ID = InvokeID.About.ToString(),
            });
            #endregion

            #region 发送模块
            // 初始化
            RegisterItem(new Send_NewViewModel(Store)
            {
                DisplayName = SendStatus.New.ToString(),
                ID = InvokeID.Send_New.ToString(),
            });


            RegisterItem(new Send_PreviewViewModel(Store)
            {
                DisplayName = SendStatus.Preview.ToString(),
                ID = InvokeID.Send_Preview.ToString(),
            });


            RegisterItem(new Send_SendingViewModel(Store)
            {
                DisplayName = SendStatus.Sending.ToString(),
                ID = InvokeID.Send_Sending.ToString(),
            });


            RegisterItem(new Send_SentViewModel(Store)
            {
                DisplayName = SendStatus.Sent.ToString(),
                ID = InvokeID.Send_Sent.ToString(),
            });
            #endregion

            // 激活
            InvokeTo(new InvokeParameter() { InvokeId = "个人中心" });

            base.OnInitialActivate();
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            // 将 view 保存到全局
            Store.MainWindow = this.View as WindowX;
            Store.MainScreen = this;

            // 另起一个线程检查更新
            Thread thread = new Thread(() =>
            {
                CheckVersion();
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private async void CheckVersion()
        {
            // 从服务器获取更新的json文件
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");
            try
            {
                HttpResponseMessage response = await client.GetAsync(Store.ConfigManager.AppConfig.GitHubVersionUrl);

                if (response.StatusCode != HttpStatusCode.OK) return;

                string result = await response.Content.ReadAsStringAsync();
                // 获取最近的地址
                Latest latest = ((JObject)JsonConvert.DeserializeObject(result)).ToObject<Latest>();
                if (latest == null || latest.assets == null || latest.assets.Length < 1) return;

                // 读取配置文件
                string configDownloadUrl = latest.assets.Where(item => item.name == Store.ConfigManager.AppConfig.VersionConfigName).FirstOrDefault().browser_download_url;

                // 下载配置文件
                HttpResponseMessage response2 = await client.GetAsync(configDownloadUrl);
                if (response2.StatusCode != HttpStatusCode.OK) return;

                // 读取配置文件
                string result2 = await response2.Content.ReadAsStringAsync();
                VersionInfo latestConfig = ((JObject)JsonConvert.DeserializeObject(result2)).ToObject<VersionInfo>();

                //VersionInfo latestConfig = new VersionInfo()
                //{
                //    version = "1.0.0",
                //};
                //latestConfig.downloadedUrls = new DownloadedUrl[2]
                //{
                //    new DownloadedUrl(){name="baiduyun",browser_download_url="https://galensgan.github.io/"},
                //    new DownloadedUrl(){name="github",browser_download_url="https://galensgan.github.io/"},
                //};

                // 写入到全局数据中心
                Store.VersionInfo = latestConfig;

                // 比较版本号
                System.Version serviceVersion = new Version(latestConfig.version);
                System.Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                if (serviceVersion > currentVersion)
                {

                    // 显示到界面
                    NewVersion = latestConfig.version;
                    IsNewVersion = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #region 新版本号
        public Visibility IsNewVersion { get; set; } = Visibility.Collapsed;
        public string NewVersion { get; set; } = string.Empty;

        public void DownLoadNewVersion()
        {
            // 如果只有一个链接，就直接打开
            if (Store.VersionInfo.downloadedUrls.Length == 1)
            {
                System.Diagnostics.Process.Start(Store.VersionInfo.downloadedUrls[0].browser_download_url);
                return;
            }

            // 如果有多个链接，显示窗体，让用户选择
            Store.MainWindow.IsMaskVisible = true;
            DownloadedURLsListViewModel downloadVM = new DownloadedURLsListViewModel(Store);
            Store.WindowManager.ShowWindow(downloadVM);
            Store.MainWindow.IsMaskVisible = false;
        }
        #endregion
    }
}
