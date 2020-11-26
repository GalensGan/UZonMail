using Panuon.UI.Silver;
using SendMultipleEmails.Database;
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
using GalensSDK.Encrypt;
using GalensSDK.TimeEx;

namespace SendMultipleEmails.Pages
{
    public class LoginViewModel : ScreenChild
    {
        public string UserId { get; set; }
        public string Password { get; set; }

        public LoginViewModel(Store store) : base(store)
        {
            // 加载上次登陆的用户
            this.UserId = store.ConfigManager.AppConfig.LastVisitUserId;
        }

        public void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            Password = pb.Password;
        }

        private bool ValidateAccount()
        {
            if (string.IsNullOrEmpty(UserId))
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

            IAccountDb iAccount = Store.GetAccountDatabase<IAccountDb>();

            // 查找是否存在当前用户，如果存在，则不进行注册
            Account account = iAccount.FindOneAccount(UserId);
            if (account != null)
            {
                MessageBoxX.Show("当前用户已经存在，请更换账号注册", "注册失败");
                return;
            }

            // 开始注册
            Account newAccount = new Account()
            {
                UserId = this.UserId,
                PassWord = MD5Ex.EncryptString(this.Password),
                LastVisitTimestamp = TimeHelper.TimestampNow()
            };
            if (!iAccount.InsertAccount(newAccount))
            {
                MessageBoxX.Show("未能添加到数据库", "注册失败");
                return;
            }

            MessageBoxX.Show("注册成功，请登陆", "注册");
        }

        public void Login()
        {
            if (!ValidateAccount()) return;

            // 查找当前账户
            Account result = Store.GetAccountDatabase<IAccountDb>().FindOneAccount(UserId);
            if (result == null)
            {
                MessageBoxX.Show("用户不存在，请重新输入", "验证失败");
                return;
            }

            // 验证密码
            if (result.PassWord != MD5Ex.EncryptString(this.Password))
            {
                MessageBoxX.Show("密码错误，请重新输入", "验证失败");
                return;
            }

            // 更改服务器最后访问时间
            result.LastVisitTimestamp = TimeHelper.TimestampNow();
            Store.GetAccountDatabase<IAccountDb>().UpdateAccount(result);

            // 加载数据
            Store.LoginAccount(result);

            this.RequestClose(true);
        }

        // 退出
        public void Quite()
        {
            System.Environment.Exit(0);
        }
    }
}
