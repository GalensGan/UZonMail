using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using Stylet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SendMultipleEmails.Pages
{
    class ReceiversViewModel:ScreenChild
    {
        public ReceiversViewModel(Store store) : base(store) { }
        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            DataSource = new BindingSource();
            DataSource.DataSource = Store.PersonalDataManager.PersonalData.receivers;
        }

        public string ExcelFullPath { get; set; } = string.Empty;

        public BindingSource DataSource { get; set; }

        public bool CanAddReceiver { get; set; } = true;

        public void AddReceiver()
        {
            AddReceiverViewModel vm = new AddReceiverViewModel(Store);
            Store.ShowDialogWithMask(vm);
        }

        public bool CanAddReceivers { get; set; } = true;
        public void AddReceivers()
        {
            AddReceiversViewModel vm = new AddReceiversViewModel(Store);
            Store.ShowDialogWithMask(vm);
        }

        public Sender SelectedSender { get; set; }
        public void DeleteReceiver(DataRowView row)
        {
            MessageBoxResult result = MessageBoxX.Show("是否删除收件人?", "信息确认", null, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) return;

            // 删除发件人
            row.Delete();
            Store.PersonalDataManager.Save();
        }

        public void ClearAll()
        {
            MessageBoxResult result = MessageBoxX.Show("是否清空所有收件人?", "信息确认", null, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) return;

            // 删除发件人
            Store.PersonalDataManager.ClearAllReceiver();
            Store.PersonalDataManager.Save();
        }

        public string FilterText { get; set; } = "";

        public void Filter()
        {
            // 获取所有的列头
            List<string> names = Store.PersonalDataManager.GetTableNames(Store.PersonalDataManager.PersonalData.receivers);
            string sql = string.Empty;
            for (int i = 0; i < names.Count; i++)
            {
                if (i == 0)
                {
                    sql = string.Format("{0} LIKE '*{1}*'", names[i], FilterText);
                }
                else sql += string.Format(" OR {0} LIKE '*{1}*'", names[i], FilterText);
            }

            DataSource.Filter = sql;
        }
    }
}
