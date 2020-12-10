using Panuon.UI.Silver;
using SendMultipleEmails.Database;
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
    class Senders_AddViewModel:ScreenChild
    {
        public Senders_AddViewModel(Store store) : base(store) 
        {
            Sender = new Sender();
        }

        public Sender Sender { get; set; }

        public void AddSender()
        {
            if (!Sender.Validate(null)) return;

            // 查找是否重复
            Sender existSender = Store.GetUserDatabase<ISenderDb>().FindOneSenderByName(Sender.UserId);

            if (existSender==null)
            {
                MessageBoxX.Show("添加的发件人已经存在，请勿重复添加","温馨提示");
                return;
            }

            // 添加用户
            Store.GetUserDatabase<ISenderDb>().InsertSender(Sender);

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
