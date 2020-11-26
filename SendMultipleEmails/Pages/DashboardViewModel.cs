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
    class DashboardViewModel: ScreenChild
    {
        public DashboardViewModel(Store store) : base(store) { }

        // 退出
        public void Exit()
        {
            Store.Close();
        }

        public void Unsubscribe()
        {
            MessageBoxResult result = MessageBoxX.Show("注销账户将清空账户内产生的所有数据，是否继续？", "注销确认", null, System.Windows.MessageBoxButton.YesNo);
            if (result == MessageBoxResult.OK)
            {
                Directory.Delete(Store.ConfigManager.AppConfig.UserDataDir);
                // 退出
                Exit();
            }
        }

        public string UserName
        {
            get => Store.AccountManager.CurrentAccount.UserName;
        }
    }
}
