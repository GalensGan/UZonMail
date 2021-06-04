using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SendMultipleEmails.Pages
{
    public class SaveAsInputViewModel : ScreenChild
    {
        private string _content;
        public SaveAsInputViewModel(Store store, string content) : base(store)
        {
            _content = content;
        }

        public string TemplateName { get; set; } = string.Empty;

        public void Confirm()
        {
            if (string.IsNullOrEmpty(TemplateName))
            {
                Store.ShowInfo("请输入模板名称", "模板名为空");
                return;
            }

            // 保存数据
            string newPath = Store.ConfigManager.AppConfig.UserTemplateDir + "\\" + TemplateName;
            newPath = newPath.Replace(".html", "") + ".html";

            // 检查是否存在
            if (File.Exists(newPath)){
                Store.ShowError("模板已经存在，请重新输入", "模板重复");
                return;
            }

            Store.TemplateManager.Save(newPath, _content);

            this.InvokeTo(new GalensSDK.StyletEx.InvokeParameter() { Arg = newPath });

            // 退出
            this.RequestClose(true);
        }

        public void Quite()
        {
            this.RequestClose(false);
        }
    }
}
