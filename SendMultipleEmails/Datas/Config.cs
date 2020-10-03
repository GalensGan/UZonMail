using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
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

        // 账户保存目录
        public string accountsPath = "Data\\accounts.json";

        public string aboutMePath = "Data\\AboutMe.md";

        public string userPersonalDataFileName = "personalData.json";

        public bool isAutoSave = false;

        [JsonIgnore]
        public string UserTemplateDir { get; set; }
        [JsonIgnore]
        public string UserDataDir { get; set; }
        [JsonIgnore]
        public string UserHistoryPath { get; set; }
    }
}
