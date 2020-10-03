using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SendMultipleEmails.Pages
{
    public class LoginViewModel : ScreenChild
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public LoginViewModel(Store store) : base(store)
        {
            // 加载上次登陆的用户
            if (store.AccountManager.LastAccount != null)
            {
                this.UserName = store.AccountManager.LastAccount.userName;
            }
        }

        public void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            Password = pb.Password;
        }

        private bool ValidateAccount()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBoxX.Show("请输入用户名", "温馨提示");
                return false;
            }

            if (string.IsNullOrEmpty(Password))
            {
                MessageBoxX.Show("请输入密码", "温馨提示");
                return false;
            }
            return true;
        }

        public void Register()
        {
            if (!ValidateAccount()) return;

            Tuple<bool, string> tuple = Store.AccountManager.Register(new Account() { userName = UserName, passWord = Password });

            if (!tuple.Item1)
            {
                MessageBoxX.Show(tuple.Item2, "注册");
                return;
            };

            MessageBoxX.Show("注册成功，请登陆", "注册");
        }

        public void Login()
        {
            if (!ValidateAccount()) return;

            // 保存数据
            Account loginAccount = new Account() { userName = UserName, passWord = Password };
            Tuple<bool, string> tuple = Store.AccountManager.Validate(loginAccount);
            if (!tuple.Item1)
            {
                MessageBoxX.Show(tuple.Item2, "验证失败");
                return;
            };

            // 记住登陆的账户
            Store.AccountManager.RememberAccount(loginAccount);

            Store.AccountManager.Save();

            // 加载数据
            Store.LoadDataForUser();
            this.RequestClose(true);
        }

        // 退出
        public void Quite()
        {
            System.Environment.Exit(0);
        }
    }
}
