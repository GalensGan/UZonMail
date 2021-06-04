using NPOI.OpenXmlFormats.Dml;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Pages
{
    public class SendSettingsViewModel:ScreenChild
    {
        public SendSettingsViewModel(Store store) : base(store) { }

        public double SendInterval
        {
            get
            {
                return Store.ConfigManager.PersonalConfig.SendInterval;
            }
            set
            {
                Store.ConfigManager.PersonalConfig.SendInterval = Math.Round(value,1);
                base.NotifyOfPropertyChange();
            }
        }

        public double SendIntervalRandom
        {
            get
            {
                return Store.ConfigManager.PersonalConfig.SendIntervalRandom;
            }
            set
            {
                Store.ConfigManager.PersonalConfig.SendIntervalRandom = Math.Round(value, 1);
                base.NotifyOfPropertyChange();
            }
        }
    }
}
