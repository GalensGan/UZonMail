using GalensSDK.DatatableEx;
using GalensSDK.Enumerable;
using Panuon.UI.Silver;
using SendMultipleEmails.Database;
using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
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
    public class SendersViewModel : ScreenChild
    {
        public SendersViewModel(Store store) : base(store) { }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            SenderList = new BindingSource();

            // 从数据库中获取数据
            List<Sender> senders = Store.GetUserDatabase<ISenderDb>().FindAllSenders().ToList();
            SenderList.DataSource = senders.ConvertToDt();
        }

        public string ExcelFullPath { get; set; } = string.Empty;

        public BindingSource SenderList { get; set; }

        public void AddSender()
        {
            Senders_AddViewModel vm = new Senders_AddViewModel(Store);
            bool? result = Store.ShowDialogWithMask(vm);
            if (result == null || !(bool)result) return;

            // 添加完成后，要重新更新
            UpdateGrid();
        }

        public bool CanAddSenders { get; set; } = true;
        public void AddSenders()
        {
            Senders_ImportViewModel vm = new Senders_ImportViewModel(Store);
            bool? result = Store.ShowDialogWithMask(vm);
            if (result == null || !(bool)result) return;

            // 添加完成后，要重新更新
            UpdateGrid();
        }

        public Sender SelectedSender { get; set; }
        public void Delete(DataRowView row)
        {
            Sender sender = row.Row.ConvertToModel<Sender>();
            MessageBoxResult result = MessageBoxX.Show(Store.MainWindow,string.Format("是否删除发件人:{0}?", sender.UserId), "信息确认", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) return;

            // 删除发件人
            row.Delete();
            // 删除数据库中数据
            Store.GetUserDatabase<ISenderDb>().DeleteSender(sender.Id);
        }

        // 编辑
        public void Edit(DataRowView row)
        {
            Senders_AddViewModel vm = new Senders_AddViewModel(Store,row);
            bool? result = Store.ShowDialogWithMask(vm);
            if (result == null || !(bool)result) return;

            // 完成后，要重新更新
            UpdateGrid();
        }

        // 设置成发件人
        public void AsSender(DataRowView row)
        {
            Sender sender = row.Row.ConvertToModel<Sender>();
            sender.IsAsSender = true;
            if (Store.GetUserDatabase<ISenderDb>().UpdateSender(sender))
            {
                row.Row[FieldKey.IsAsSender.ToString()] = true;
            }
        }

        // 取消发件人设置
        public void CancleSender(DataRowView row)
        {
            Sender sender = row.Row.ConvertToModel<Sender>();
            sender.IsAsSender = false;
            if (Store.GetUserDatabase<ISenderDb>().UpdateSender(sender))
            {
                row.Row[FieldKey.IsAsSender.ToString()] = false;
            }
        }

        public void UpdateGrid()
        {            
            SenderList.DataSource = Store.GetUserDatabase<ISenderDb>().FindAllSenders().ToList().ConvertToDt();
        }

        public string FilterText { get; set; } = "";

        public void Filter()
        {
            // 获取所有的列头
            List<string> names = (SenderList.DataSource as DataTable).GetColumnNamesOfStringColumn();
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
