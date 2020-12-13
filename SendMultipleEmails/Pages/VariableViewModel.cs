using GalensSDK.DatatableEx;
using GalensSDK.LiteDbEx;
using LiteDB;
using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
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
    class VariableViewModel : ScreenChild
    {
        public VariableViewModel(Store store) : base(store) { }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            Variables = new BindingSource
            {
                // 获取绑定的数据
                DataSource = Store.GetCollection(DatabaseName.Variable.ToString()).FindAll().ToDataTable()
            };
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            // 更新功能操作键位置
            VariableView sdv = this.View as VariableView;
            BsonDocument bdoc = Store.GetCollection(DatabaseName.Variable.ToString()).FindOne(Query.Not(FieldKey._id.ToString(), new BsonValue(string.Empty)));
            if (bdoc != null) sdv.OperationRow.DisplayIndex = bdoc.Keys.Count;
        }

        public BindingSource Variables { get; set; }

        public void ReadData()
        {
            Screen s = new Variable_AddViewModel(Store);
            bool? reuslt = Store.ShowDialogWithMask(s);
            if (!(bool)reuslt) return;

            // 更新 Data
            Variables = new BindingSource
            {
                DataSource = Store.GetCollection(DatabaseName.Variable.ToString()).FindAll().ToDataTable()
            };
            // 更新功能操作键位置
            VariableView sdv = this.View as VariableView;
            sdv.OperationRow.DisplayIndex = Store.GetCollection(DatabaseName.Variable.ToString()).FindOne(Query.Not(FieldKey._id.ToString(), new BsonValue(string.Empty))).Keys.Count;
        }

        public void DeleteData(System.Data.DataRowView drv)
        {
            // 找到姓名或者Name
            MessageBoxResult result = Store.ShowWarning("是否删除收件人数据?", "信息确认");
            if (result == MessageBoxResult.Cancel) return;

            // 删除数据库中的数据
            Store.GetCollection(DatabaseName.Variable.ToString()).Delete(new BsonValue(drv.Row[FieldKey._id.ToString()]));

            // 删除发件人
            drv.Delete();
        }

        public string FilterText { get; set; } = "";

        public void Filter()
        {
            // 获取所有的列头
            List<string> names = (Variables.DataSource as DataTable).GetColumnNamesOfStringColumn();
            string sql = string.Empty;
            for (int i = 0; i < names.Count; i++)
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
