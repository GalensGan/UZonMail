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
                return Store.PersonalDataManager.PersonalData.sendInterval;
            }
            set
            {
                base.SetAndNotify(ref Store.PersonalDataManager.PersonalData.sendInterval, value);
            }
        }

        public double SendIntervalRandom
        {
            get
            {
                return Store.PersonalDataManager.PersonalData.sendIntervalRandom;
            }
            set
            {
                base.SetAndNotify(ref Store.PersonalDataManager.PersonalData.sendIntervalRandom, value);
            }
        }
    }
}
