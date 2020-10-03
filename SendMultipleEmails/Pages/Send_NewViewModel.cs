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
