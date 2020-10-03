using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SendMultipleEmails.Pages
{
    public class SaveAsInputViewModel:ScreenChild
    {
        public SaveAsInputViewModel(Store store) : base(store) { }

        public string TemplateName { get; set; } = string.Empty;

        public void Confirm()
        {
            if (string.IsNullOrEmpty(TemplateName))
            {
                MessageBoxX.Show("请输入模板名称", "模板名为空");
                return;
            }

            // 保存模板名称
            Store.TemplateName = TemplateName;

            // 退出
            this.RequestClose(true);
        }

        public void Quite()
        {
            this.RequestClose(false);
        }
    }
}
