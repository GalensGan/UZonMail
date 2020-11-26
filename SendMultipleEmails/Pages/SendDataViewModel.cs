using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using Stylet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Screen = Stylet.Screen;

namespace SendMultipleEmails.Pages
{
    class SendDataViewModel:ScreenChild
    {
        public SendDataViewModel(Store store) : base(store) { }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            Variables = new BindingSource
            {
                DataSource = Store.PersonalDataManager.PersonalData.variablesTable
            };
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            // 更新功能操作键位置
            SendDataView sdv = this.View as SendDataView;
            sdv.OperationRow.DisplayIndex = Store.PersonalDataManager.PersonalData.variablesTable.Columns.Count;
        }

        public BindingSource Variables { get; set; }

        public void ReadData()
        {
            Screen s = new AddVariableViewModel(Store);
            Store.WindowManager.ShowDialog(s);


            // 更新 Data
            Variables = new BindingSource
            {
                DataSource = Store.PersonalDataManager.PersonalData.variablesTable
            };
            // 更新功能操作键位置
            SendDataView sdv = this.View as SendDataView;
            sdv.OperationRow.DisplayIndex = Store.PersonalDataManager.PersonalData.variablesTable.Columns.Count;
        }

        public void DeleteData(System.Data.DataRowView drv)
        {
            // 找到姓名或者Name
            MessageBoxResult result = MessageBoxX.Show("是否删除收件人数据?" , "信息确认", null, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) return;

            // 删除发件人
            drv.Delete();
            Store.PersonalDataManager.Save();
            //Store.PersonalDataManager.RemoveVariable(userName);
        }

        public string FilterText { get; set; } = "";

        public void Filter()
        {
            // 获取所有的列头
            List<string> names = Store.PersonalDataManager.GetTableNames(Store.PersonalDataManager.PersonalData.variablesTable);
            string sql = string.Empty;
            for(int i = 0; i < names.Count; i++)
            {
                if (i == 0)
                {
                    sql = string.Format("{0} LIKE '*{1}*'", names[i], FilterText);
                }
                else sql += string.Format(" OR {0} LIKE '*{1}*'", names[i], FilterText);
            }

            Variables.Filter = sql;
        }
    }
}
