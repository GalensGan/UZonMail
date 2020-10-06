using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats.Dml;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using SendMultipleEmails.Datas;
using SendMultipleEmails.ResponseJson;
using Stylet;

namespace SendMultipleEmails.Pages
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        #region 属性

        private bool _isDashboard = true;
        public bool IsDashboard
        {
            get => _isDashboard;
            set
            {
                base.SetAndNotify(ref _isDashboard, value);
                if (value) ActiveItemByDisplayName("个人中心");
            }
        }

        private bool _isSendSettings = false;
        public bool IsSendSettings
        {
            get => _isSendSettings;
            set
            {
                base.SetAndNotify(ref _isSendSettings, value);
                if (value) ActiveItemByDisplayName("发送设置");
            }
        }

        private bool _isSenders = false;
        public bool IsSenders
        {
            get => _isSenders;
            set
            {
                base.SetAndNotify(ref _isSenders, value);
                if (value) ActiveItemByDisplayName("发件人");
            }
        }

        private bool _isReceivers = false;
        public bool IsReceivers
        {
            get => _isReceivers;
            set
            {
                SetAndNotify(ref _isReceivers, value);
                if (value) ActiveItemByDisplayName("收件人");
            }
        }

        private bool _isImportData;
        public bool IsImportData
        {
            get => _isImportData;
            set
            {
                base.SetAndNotify(ref _isImportData, value);
                if (value) ActiveItemByDisplayName("导入数据");
            }
        }

        private bool _isTemplate = false;
        public bool IsTemplate
        {
            get => _isTemplate;
            set
            {
                base.SetAndNotify(ref _isTemplate, value);
                if (value) ActiveItemByDisplayName("模板");
            }
        }

        private bool _isSend = false;
        public bool IsSend
        {
            get => _isSend;
            set
            {
                base.SetAndNotify(ref _isSend, value);
                if (value) ActiveItemByDisplayName("发送");
            }
        }

        private bool _isLog = false;
        public bool IsLog
        {
            get => _isLog;
            set
            {
                SetAndNotify(ref _isLog, value);
                if (value) ActiveItemByDisplayName("日志");
            }
        }

        private bool _isAboutMe = false;
        public bool IsAboutMe
        {
            get => _isAboutMe;
            set
            {
                SetAndNotify(ref _isAboutMe, value);
                if (value) ActiveItemByDisplayName("关于");
            }
        }

        public Store Store { get; private set; }

        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        #endregion

        #region 私有字段
        private IWindowManager _windowManager;
        #endregion
        private void ActiveItemByDisplayName(string displayName)
        {
            Screen screen = this.Items.Where(item => item.DisplayName == displayName).FirstOrDefault();
            if (screen != null) this.ActivateItem(screen);
        }


        public ShellViewModel(IWindowManager windowManager)
        {
            // 进行环境检查，暂时取消

            _windowManager = windowManager;
            Store = new Store(this);
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (Store.ConfigManager.AppConfig.isAutoSave)
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
                Store.ConfigManager.AppConfig.isAutoSave = true;
                Store.Save();
            }
        }

        protected override void OnInitialActivate()
        {
            Screen loginVM = new LoginViewModel(Store);
            _windowManager.ShowDialog(loginVM);

            this.Items.Add(new DashboardViewModel(Store, _windowManager)
            {
                DisplayName = "个人中心",
            });

            this.Items.Add(new SendSettingsViewModel(Store)
            {
                DisplayName = "发送设置",
                IconName = "&#xf015;"
            });

            this.Items.Add(new SendersViewModel(Store, _windowManager)
            {
                DisplayName = "发件人",
                IconName = "&#xf015;"
            });

            this.Items.Add(new ReceiversViewModel(Store, _windowManager)
            {
                DisplayName = "收件人",
                IconName = "&#xf015;"
            });

            this.Items.Add(new SendDataViewModel(Store, _windowManager)
            {
                DisplayName = "导入数据",
                IconName = "&#xf015;"
            });

            this.Items.Add(new TemplateViewModel(Store, _windowManager)
            {
                DisplayName = "模板",
                IconName = "&#xf015;"
            });

            this.Items.Add(new SendViewModel(Store, _windowManager)
            {
                DisplayName = "发送"
            });

            this.Items.Add(new LogViewModel(Store)
            {
                DisplayName = "日志"
            });

            this.Items.Add(new AboutMeViewModel(Store)
            {
                DisplayName = "关于",
            });

            // 激活
            ActiveItemByDisplayName("个人中心");
            base.OnInitialActivate();
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

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
                HttpResponseMessage response = await client.GetAsync(Store.ConfigManager.AppConfig.gitHubVersionUrl);

                if (response.StatusCode != HttpStatusCode.OK) return;

                string result = await response.Content.ReadAsStringAsync();
                // 获取最近的地址
                Latest latest = ((JObject)JsonConvert.DeserializeObject(result)).ToObject<Latest>();
                if (latest == null || latest.assets == null || latest.assets.Length < 1) return;

                // 读取配置文件
                string configDownloadUrl = latest.assets.Where(item => item.name == Store.ConfigManager.AppConfig.versionConfigName).FirstOrDefault().browser_download_url;

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
                if (serviceVersion > currentVersion)                {

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
            DownloadedURLsListViewModel downloadVM = new DownloadedURLsListViewModel(Store);
            _windowManager.ShowWindow(downloadVM);
        }
        #endregion
    }
}
