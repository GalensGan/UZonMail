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
        private const string _configDir = "Data\\config.json";

        public ConfigManager() : base(null)
        {
            // 从特定位置读取配置，然后发送给父类
            if (!File.Exists(_configDir))
            {
                Config = new Config();
            }
            else
            {
                // 读取配置文件
                Config = this.ReadData<Config, JObject>(_configDir);
            }
        }

        public Config AppConfig => Config;

        public override bool Save()
        {
            this.Save(_configDir, Config);
            return true;
        }
    }
}
