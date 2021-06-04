using SendMultipleEmails.Datas;
using SendMultipleEmails.ResponseJson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace SendMultipleEmails.Pages
{
    class DownloadedURLsListViewModel:ScreenChild
    {
        public BindingSource DownloadSources { get; set; }
        public DownloadedURLsListViewModel(Store store) : base(store)
        {
            // 初始化数据
            DownloadSources = new BindingSource();

            store.VersionInfo.downloadedUrls.ToList().ForEach(item => DownloadSources.Add(item));
        }

        public void Download(DownloadedUrl row)
        {
            System.Diagnostics.Process.Start(row.browser_download_url);
            this.RequestClose();
        }
    }
}
