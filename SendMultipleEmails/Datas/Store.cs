using LiteDB;
using Panuon.UI.Silver;
using SendMultipleEmails.Database;
using SendMultipleEmails.ResponseJson;
using Stylet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SendMultipleEmails.Datas
{
    public class Store
    {
        #region 配置
        public ConfigManager ConfigManager { get; private set; }
        #endregion


        #region 构造函数
        public IWindowManager WindowManager { get; private set; }

        // 主窗体
        public WindowX MainWindow { get; set; }

        public Screen MainScreen { get; set; }

        public Store(IWindowManager windowManager)
        {
            WindowManager = windowManager;

            // 初始化配置文件
            ConfigManager = new ConfigManager();

            // 初始化账户数据库
            _accountdatabase = new LiteDbConcrete(ConfigManager.AppConfig.AccountDatabaseFullName);
        }
        #endregion


        #region // 登陆,登陆之后需要加载其它用户数据
        /// <summary>
        /// 登陆的账户
        /// </summary>
        public Account CurrentAccount => ConfigManager.AppConfig.CurrentAccount;
        /// <summary>
        /// 登陆后，将登陆的用户保存
        /// </summary>
        /// <param name="account"></param>
        public void LoginAccount(Account account)
        {
            // 将登录的数据写到配置中
            ConfigManager.AppConfig.CurrentAccount = account;
            ConfigManager.AppConfig.LastVisitUserId = account.UserId;
            ConfigManager.Save();

            // 打开个人数据库
            _userDatabase = new LiteDbConcrete(ConfigManager.AppConfig.UserDatabaseFullName);

            // 读取个人的配置
            ConfigManager.ReadPersonalConfig(ConfigManager.AppConfig.UserSettings);

            // 加载模块库
            TemplateManager = new TemplateManager(ConfigManager.AppConfig);

        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 保存数据库之外的数据
        /// </summary>
        public void Save()
        {
            ConfigManager.Save();
        }

        /// <summary>
        /// 关闭主窗体
        /// </summary>
        public void Close()
        {
            this.MainScreen.RequestClose();
        }

        /// <summary>
        /// 打开模态对话框
        /// </summary>
        /// <param name="screen"></param>
        /// <returns></returns>
        public bool? ShowDialogWithMask(object screen)
        {
            MainWindow.IsMaskVisible = true;
            bool? result = this.WindowManager.ShowDialog(screen);
            MainWindow.IsMaskVisible = false;

            return result;
        }
        #endregion


        #region // 模板
        public TemplateManager TemplateManager { get; private set; }
        #endregion

        #region 临时数据
        public ConcurrentQueue<Tuple<Person, string>> QueueReceivers;

        public string MainTitle { get; set; } = string.Empty;

        public string TemplateName { get; set; } = string.Empty;

        /// <summary>
        /// 版本信息
        /// </summary>
        public VersionInfo VersionInfo { get; set; }
        #endregion

        #region 数据库
        /// <summary>
        /// 获取相应类型的数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAccountDatabase<T>() where T:IAccountDb
        {
            return (T)_accountdatabase;
        }

        private IDatabase _accountdatabase;

        public T GetUserDatabase<T>()
        {
            return (T)_userDatabase;
        }

        public ILiteCollection<BsonDocument> GetCollection(string collectionName)
        {
            return _userDatabase.GetCollection(collectionName);
        }

        private IDatabase _userDatabase;
        #endregion

        #region 提示窗体
        public MessageBoxResult ShowInfo(string text,string caption)
        {
            return MessageBoxX.Show(MainWindow, text, caption, MessageBoxButton.OK, MessageBoxIcon.Info);
        }

        public MessageBoxResult ShowSuccess(string text, string caption)
        {
            return MessageBoxX.Show(MainWindow, text, caption, MessageBoxButton.OK, MessageBoxIcon.Success);
        }

        public MessageBoxResult ShowError(string text,string caption)
        {
            return MessageBoxX.Show(MainWindow, text, caption, MessageBoxButton.OK, MessageBoxIcon.Error);
        }

        public MessageBoxResult ShowWarning(string text, string caption)
        {
            return MessageBoxX.Show(MainWindow, text, caption, MessageBoxButton.OKCancel, MessageBoxIcon.Warning,DefaultButton.CancelNo);
        }
        #endregion
    }
}
