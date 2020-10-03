using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Screen = Stylet.Screen;

namespace SendMultipleEmails.Pages
{
    class DashboardViewModel: Conductor<Screen>.Collection.OneActive
    {
        private Store _store;
        private IWindowManager _windowManager;
        public DashboardViewModel(Store store,IWindowManager windowManager) 
        {
            _store = store;
            _windowManager = windowManager;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            this.Items.Add(new Dashboard_MainViewModel(_store)
            {
                DisplayName = "main"
            });

            // 激活
            ActiveItemByDisplayName("main");
        }

        private void ActiveItemByDisplayName(string displayName)
        {
            Screen screen = this.Items.Where(item => item.DisplayName == displayName).FirstOrDefault();
            if (screen is ScreenChild sc)
            {
                this.ActivateItem(sc);
            }
        }

        // 退出
        public void Exit()
        {
            _store.Close();
        }

        public void Unsubscribe()
        {
            MessageBoxResult result = MessageBoxX.Show("注销账户将清空账户内产生的所有数据，是否继续？", "注销确认", null, System.Windows.MessageBoxButton.YesNo);
            if (result == MessageBoxResult.OK)
            {
                // 清除账户数据
                Directory.Delete(this._store.ConfigManager.AppConfig.UserDataDir);

                // 退出
                Exit();
            }
        }

        public string UserName
        {
            get => _store.AccountManager.CurrentAccount.userName;
        }
    }
}
