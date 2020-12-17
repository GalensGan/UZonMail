using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Pages
{
    class Send_IndexViewModel:ScreenChild
    {
        public Send_IndexViewModel(Store store) : base(store) { }


        public string NewTip { get; set; } = "新建发送";

        public void New()
        {
            InvokeTo(new GalensSDK.StyletEx.InvokeParameter() { InvokeId = InvokeID.Send_New.ToString() });
        }

        public void History()
        {
            InvokeTo(new GalensSDK.StyletEx.InvokeParameter() { InvokeId = InvokeID.Send_Sent.ToString() });
        }
    }
}
