using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    /// <summary>
    /// 基本数据设置
    /// </summary>
    public class Config
    {
        public int smtpPort = 25;

        private string _globalData = "Data";

        // 模板位置
        public string templateDir = "Template";

        // 日志位置
        public string logDir= "Log";

        // 个人数据位置
        public string userDataParent = "UserData";

        // 数据库位置
        public string databaseFilePath = "Data\\data.db";

        // 账户保存目录
        public string accountsPath = "Data\\accounts.json";

        // 是否记住上次登陆的账户
        public bool isRememberLoginInfo = false;

        public string aboutMePath = "https://noctiflorous.gitee.io/2020/10/05/README/";

        public string userPersonalDataFileName = "personalData.json";

        public bool isAutoSave = false;

        // 程序更新验证位置
        public string gitHubVersionUrl = "https://api.github.com/repos/GalensGan/SendMultipleEmails/releases/latest";

        public string giteeVersionUrl = "";

        public string versionConfigName = "latest.json";

        #region 不保存的属性
        [JsonIgnore]
        public Account CurrentAccount { get; set; }
        [JsonIgnore]
        public string UserTemplateDir
        {
            get
            {
                return string.Format("{0}\\{1}\\{2}", userDataParent, CurrentAccount.UserName, "Template");
            }
        }
        [JsonIgnore]
        public string UserDataDir
        {
            get
            {
                return string.Format("{0}\\{1}\\{2}", userDataParent, CurrentAccount.UserName, "Data");
            }
        }
        #endregion

    }
}
