using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Pages
{
    class AboutMeViewModel:ScreenChild
    {
        public string ReadMeUrl { get; set; } = "https://noctiflorous.gitee.io/2020/10/05/README/";

        public AboutMeViewModel(Store store) : base(store) { }

        protected override void OnInitialActivate()
        {
            // 读取配置文件
            ReadMeUrl = this.Store.ConfigManager.AppConfig.aboutMePath;
        }
    }
}
