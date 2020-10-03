using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using SendMultipleEmails.Datas;
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
    }
}
