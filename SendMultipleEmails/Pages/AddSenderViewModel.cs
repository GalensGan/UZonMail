using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SendMultipleEmails.Pages
{
    class AddSenderViewModel:ScreenChild
    {
        public AddSenderViewModel(Store store) : base(store) 
        {
            Sender = new Sender();
        }

        public Sender Sender { get; set; }

        public void AddSender()
        {
            if (!Sender.Validate(null)) return;

            if (!Store.PersonalDataManager.AddSender(Sender,true))
            {
                MessageBoxX.Show("添加的发件人已经存在，请不要重复添加","温馨提示");
                return;
            }

            this.RequestClose(true);
        }

        public void Quite()
        {
            this.RequestClose(false);
        }

        public void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            Sender.Password = pb.Password;
        }
    }
}
