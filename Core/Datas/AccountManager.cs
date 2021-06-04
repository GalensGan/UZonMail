using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SendMultipleEmails.Datas
{
    // 用户登陆数据
    public class AccountManager : ManagerBase
    {
        private Accounts _accounts = null;
        public Account CurrentAccount { get; private set; }
        public AccountManager(DefaultConfig config) : base(config)
        {
            // 从数据库中读取数据
            Accounts accounts = ReadData<Accounts, JObject>(config.accountsPath);
            if (accounts != null)
            {
                // 说明有
                _accounts = accounts;
            }
            else
            {
                _accounts = new Accounts();
                // 初始化其它
                _accounts.accounts = new List<Account>();
            }
        }

        public Account LastAccount
        {
            get
            {
                return _accounts.lastAccount;
            }
        }

        public override bool Save()
        {
            Save(Config.accountsPath, _accounts);
            return true;
        }

        public Tuple<bool, string> Register(Account account)
        {
            Account existAccount = _accounts.accounts.Where(item => item.UserId == account.UserId).FirstOrDefault();
            if (existAccount != null)
            {
                return new Tuple<bool, string>(false, "账户已经存在");
            }

            Account newAccount = new Account()
            {
                UserId = account.UserId,
                PassWord = MD5Hash.Hash.Content(account.PassWord),
            };

            // 用户不存在，添加用户
            _accounts.accounts.Add(newAccount);

            // 重新保存
            Save();

            return new Tuple<bool, string>(true, "注册成功");
        }

        public Tuple<bool, string> Validate(Account account)
        {
            // 验证数据，如果不存在用户名，则添加，如果存在，则验证用户名，密码
            Account existAccount = _accounts.accounts.Where(item => item.UserId == account.UserId).FirstOrDefault();
            if (existAccount == null)
            {
                return new Tuple<bool, string>(false, "账户不存在，请注册");
            }
            else
            {
                // 用户存在时，验证密码
                if (existAccount.PassWord == MD5Hash.Hash.Content(account.PassWord))
                {
                    // 将一些数据加载到 config 中
                    CurrentAccount = account;
                    LoadLoginUserDir();
                    return new Tuple<bool, string>(true, "验证成功");
                }
                else
                {
                    return new Tuple<bool, string>(false, "密码错误");
                }
            }
        }

        public void RememberAccount(Account account)
        {
            _accounts.lastAccount = account;
        }

        private void LoadLoginUserDir()
        {
            // 计算出其它目录
            Config.UserTemplateDir = Config.userDataParent + "\\" + CurrentAccount.UserId + "\\Template";
            Config.UserDataDir = Config.userDataParent + "\\" + CurrentAccount.UserId + "\\Data";
            Config.UserHistoryPath = Config.userDataParent + "\\" + CurrentAccount.UserId + "\\Data\\history.json";
        }
    }
}
