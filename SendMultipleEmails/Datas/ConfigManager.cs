using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats.Spreadsheet;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    public class ConfigManager:ManagerBase
    {
        // 配置文件位置，不可改变
        private const string _configDir = "Data\\default.json";

        public ConfigManager() : base(null)
        {
            // 从特定位置读取配置，然后发送给父类
            if (!File.Exists(_configDir))
            {
                Config = new AppConfig();
            }
            else
            {
                // 读取配置文件
                Config = this.ReadData<AppConfig, JObject>(_configDir);
            }
        }

        public AppConfig AppConfig => Config;
        public bool SaveAppConfig()
        {
            base.Save(_configDir, Config);

            return true;
        }

        #region 个人用户配置
        private string _personalConfigPath = string.Empty;
        public void ReadPersonalConfig(string personalConfigPath)
        {
            _personalConfigPath = personalConfigPath;

            // 从特定位置读取配置，然后发送给父类
            if (!File.Exists(personalConfigPath))
            {
                PersonalConfig = new PersonalConfig();
            }
            else
            {
                // 读取配置文件
                PersonalConfig = this.ReadData<PersonalConfig, JObject>(personalConfigPath);
            }
        }
        public PersonalConfig PersonalConfig { get;private set; }

        /// <summary>
        /// 保存用户设置
        /// </summary>
        /// <returns></returns>
        public bool SavePersonalConfig()
        {
            base.Save(_personalConfigPath, PersonalConfig);

            return true;
        }
        #endregion

        /// <summary>
        /// 保存全部设置
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            this.SaveAppConfig();
            this.SavePersonalConfig();
            return true;
        }
    }
}
