using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Pages
{
    class Send_NewViewModel:SendScreenBase
    {
        public Send_NewViewModel(Store store) : base(store) { }

        protected override void OnActivate()
        {
            base.OnActivate();

            // 判断是否有发件人
            if (Store.PersonalDataManager.GetUsingSenders().Count < 1)
            {
                // 不能新建
                CanNew = false;
                NewTip = "未找到发件人,先添加发件人";
                return;
            }

            // 判断是否可以新建
            if (Store.PersonalDataManager.GetCurrentReceiverCount() < 1)
            {
                // 不能新建
                CanNew = false;
                NewTip = "未找到收件人,先添加收件人";
                return;
            }

            CanNew = true;
        }

        public string NewTip { get; set; } = "新建发送";

        public bool CanNew { get; set; } = false;
        public void New()
        {
            NextCommand(Enums.SendStatus.Preview);
        }

        public void History()
        {
            NextCommand(Enums.SendStatus.Sent);
        }
    }
}
