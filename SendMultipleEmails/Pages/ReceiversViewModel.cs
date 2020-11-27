using GalensSDK.Enumerable;
using Panuon.UI.Silver;
using SendMultipleEmails.Database;
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

            // 从数据库中获取数据
            List<Receiver> senders = Store.GetUserDatabase<IReceiverDb>().FindAllReceivers().ToList();
            DataSource.DataSource = senders.ConvertToDt();
        }

        public string ExcelFullPath { get; set; } = string.Empty;

        public BindingSource DataSource { get; set; }

        public void AddReceiver()
        {
            Receivers_AddViewModel vm = new Receivers_AddViewModel(Store);
            Store.ShowDialogWithMask(vm);
        }

        public bool CanAddReceivers { get; set; } = true;
        public void AddReceivers()
        {
            Receivers_ImportViewModel vm = new Receivers_ImportViewModel(Store);
            Store.ShowDialogWithMask(vm);
        }

        public void DeleteReceiver(DataRowView row)
        {
            MessageBoxResult result = MessageBoxX.Show(Store.MainWindow,"是否删除收件人?", "信息确认", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) return;

            // 删除收件人
            row.Delete();

            Receiver receiver = row.Row.ConvertToModel<Receiver>();
            // 从数据库中删除
            Store.GetUserDatabase<IReceiverDb>().DeleteReceiver(receiver.Id);
        }

        public void ClearAll()
        {
            MessageBoxResult result = MessageBoxX.Show(Store.MainWindow,"是否清空所有收件人?", "信息确认", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) return;

            // 删除发件人
            // 从数据库中删除
            bool dResult = Store.GetUserDatabase<IReceiverDb>().DeleteAllReceivers();
            if (dResult) DataSource.DataSource = null;
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
