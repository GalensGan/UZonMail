using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;

namespace SendMultipleEmails.Pages
{
    public class SendersViewModel:ScreenChild
    {
        public SendersViewModel(Store store) : base(store) { }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            SenderList = new BindingSource();
            SenderList.DataSource = Store.PersonalDataManager.PersonalData.senders;
        }

        public string ExcelFullPath { get; set; } = string.Empty;

        public BindingSource SenderList { get; set; }

        public bool CanAddSender{ get; set; } = true;

        public void AddSender()
        {
            CanAddSender = false;

            AddSenderViewModel vm = new AddSenderViewModel(Store);
            Store.WindowManager.ShowDialog(vm);

            CanAddSender = true;
        }

        public bool CanAddSenders { get; set; } = true;
        public void AddSenders()
        {
            CanAddSenders = false;

            AddSendersViewModel vm = new AddSendersViewModel(Store);
            Store.WindowManager.ShowDialog(vm);

            CanAddSenders = true;
        }

        public Sender SelectedSender { get; set; }
        public void DeleteSender(DataRowView row)
        {
            MessageBoxResult result = MessageBoxX.Show("是否删除发件人?" , "信息确认", null, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) return;

            // 删除发件人
            row.Delete();
            Store.PersonalDataManager.Save();
        }

        public string FilterText { get; set; } = "";

        public void Filter()
        {
            // 获取所有的列头
            List<string> names = Store.PersonalDataManager.GetTableNames(Store.PersonalDataManager.PersonalData.senders);
            string sql = string.Empty;
            for (int i = 0; i < names.Count; i++)
            {
                if (i == 0)
                {
                    sql = string.Format("{0} LIKE '*{1}*'", names[i], FilterText);
                }
                else sql += string.Format(" OR {0} LIKE '*{1}*'", names[i], FilterText);
            }

            SenderList.Filter = sql;
        }
    }
}
