using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UZonMailDesktop.MVVM;

namespace UZonMailDesktop
{
    public class MainWindowViewModel : ObservableObject
    {
        private string url;

        public string URL
        {
            get { return url; }
            set
            {
                url = value;
                NotifyOfPropertyChange(() => URL);
            }
        }

        public MainWindowViewModel()
        {
            SetURL();
        }
        
        public void SetURL()
        {
            // 获取配置
            URL = ConfigurationManager.AppSettings["url"];
        }
    }
}
