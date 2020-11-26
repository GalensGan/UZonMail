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
    public class DefaultConfig
    {
        public int smtpPort = 25;

        // 模板位置
        public string TemplateDir { get; set; } = "Template";

        /// <summary>
        /// data 数据库
        /// </summary>
        public string Data { get; set; } = "Data";

        // 日志位置
        public string LogDir { get; set; } = "Log";

        // 个人数据位置
        public string UserDataParent { get; set; } = "UserData";

        // 账户数据库
        public string AccountDatabaseFullName { get; set; } = "Data\\data.db";

        // 上次登陆的账户名
        public string LastVisitUserId { get; set; } = "";

        // 关于页面的url
        public string AboutMePath { get; set; } = "https://noctiflorous.gitee.io/2020/10/05/README/";

        #region 程序更新
        public string GitHubVersionUrl { get; set; } = "https://api.github.com/repos/GalensGan/SendMultipleEmails/releases/latest";

        public string GiteeVersionUrl { get; set; } = "";

        public string VersionConfigName { get; set; } = "latest.json";
        #endregion

        #region 不保存的属性
        [JsonIgnore]
        public Account CurrentAccount { get; set; }
        [JsonIgnore]
        public string UserTemplateDir
        {
            get
            {
                return string.Format("{0}\\{1}\\{2}", UserDataParent, CurrentAccount.UserId, TemplateDir);
            }
        }
        [JsonIgnore]
        public string UserDataDir
        {
            get
            {
                return string.Format("{0}\\{1}\\{2}", UserDataParent, CurrentAccount.UserId,Data);
            }
        }

        [JsonIgnore]
        public string UserDatabaseFullName
        {
            get
            {
                return UserDataDir + "\\data.db";
            }
        }
        #endregion

    }
}
