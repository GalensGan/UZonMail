using SendMultipleEmails.Datas;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Pages
{
    public abstract class ScreenChild:Screen
    {
        public ScreenChild(Store store)
        {
            Store = store;
        }
        public Store Store { get;private set; }
        /// <summary>
        /// 图标名称
        /// </summary>
        public string IconName { get; set; }
    }
}
