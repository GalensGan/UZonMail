using GalensSDK.Enumerable;
using Panuon.UI.Silver;
using SendMultipleEmails.Database;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SendMultipleEmails.Pages
{
    class Senders_AddViewModel:ScreenChild
    {
        public string Title { get; set; } 

        public bool IsNew { get; set; }

        public Senders_AddViewModel(Store store) : base(store) 
        {
            Sender = new Sender();
            IsNew = true;
            Title = "添加发件箱";
        }

        public Senders_AddViewModel(Store store,DataRowView row) : base(store)
        {
            // 读取数据进行展示
            Sender = row.Row.ConvertToModel<Sender>();
            IsNew = false;
            Title = "修改发件箱";
        }

        public Sender Sender { get; set; }

        public void Confirm()
        {
            if (!Sender.Validate(null)) return;

            // 查找是否重复
            Sender existSender = Store.GetUserDatabase<ISenderDb>().FindOneSenderByEmail(Sender.Email);

            if (existSender!=null && IsNew)
            {
                Store.ShowInfo("当前的发件箱已经存在，请勿重复添加", "邮箱重复");
                return;
            }

            if(!IsNew && existSender!=null && existSender.Id != Sender.Id)
            {
                Store.ShowInfo("当前的发件箱已经存在，请勿重复添加", "邮箱重复");
                return;
            }

            // 添加发件箱
            if(IsNew)Store.GetUserDatabase<ISenderDb>().InsertSender(Sender);
            else
            {
                // 修改
                bool result = Store.GetUserDatabase<ISenderDb>().UpdateSender(Sender);
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
