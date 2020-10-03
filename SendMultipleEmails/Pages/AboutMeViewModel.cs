using Markdig;
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
        public string ReadMeHtml { get; set; } = "<p>暂无</p>";

        public AboutMeViewModel(Store store) : base(store) { }

        protected override void OnInitialActivate()
        {
            // 进行初始化
            // 从当前文件中读取 readme.md
            FileInfo fileInfo = new FileInfo(Store.ConfigManager.AppConfig.aboutMePath);
            if (!fileInfo.Exists) return;

            // 读取 md 文件
            StreamReader streamReader = fileInfo.OpenText();
            string md = streamReader.ReadToEnd();
            streamReader.Close();
            // 将 md 转成 html
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            ReadMeHtml = ConvertExtendedASCII(Markdown.ToHtml(md, pipeline));           
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            AboutMeView view = (AboutMeView)this.View;
            view.Wb.DocumentText = ReadMeHtml;
        }

        private  string ConvertExtendedASCII(string HTML)
        {
            return string.Format("<meta charset='utf - 8'><div>{0}</div>", HTML);
            //string retVal = "";
            //char[] s = HTML.ToCharArray();

            //foreach (char c in s)
            //{
            //    if (Convert.ToInt32(c) > 127)
            //        retVal += "&#" + Convert.ToInt32(c) + ";";
            //    else
            //        retVal += c;
            //}

            //return retVal;            
        }
    }
}
