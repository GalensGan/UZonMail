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

namespace SendMultipleEmails.Datas
{
    public class Store
    {
        private Screen _mainScreen;
        private List<ManagerBase> _managers;

        #region 配置
        public ConfigManager ConfigManager { get; private set; }
        #endregion


        #region 构造函数
        public IWindowManager WindowManager { get; private set; }

        // 主窗体
        public WindowX MainWindow { get; set; }
        public Store(IWindowManager windowManager)
        {
            WindowManager = windowManager;

            // 初始化配置文件
            ConfigManager = new ConfigManager();

            // 初始化数据库
            _database = new LiteDbConcrete(ConfigManager.AppConfig.databaseFilePath);
        }
        #endregion


        #region // 登陆,登陆之后需要加载其它用户数据

        /// <summary>
        /// 登陆后，将登陆的用户保存
        /// </summary>
        /// <param name="account"></param>
        public void LoginAccount(Account account)
        {
            ConfigManager.AppConfig.CurrentAccount = account;
        }
        #endregion

        #region 公共方法
        public void Save()
        {
            _managers.ForEach(m => m.Save());
        }

        public void Close()
        {
            _mainScreen?.RequestClose();
        }
        #endregion


        #region // 模板
        public TemplateManager TemplateManager { get; private set; }
        #endregion


        #region // 其它用户数据
        public PersonalDataManager PersonalDataManager { get; private set; }
        #endregion


        #region // 历史记录
        public HistoryManager HistoryManager { get; set; }
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
        public T GetDatabase<T>()
        {
            return (T)_database;
        }

        private IDatabase _database;
        #endregion
    }
}
