using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Config
{
    class UserConfig
    {
        private static UserConfig _instance;
        public static UserConfig Instance
        {
            get
            {
                if (_instance == null) _instance = new UserConfig();

                return _instance;
            }
        }
        private UserConfig()
        {
            // 读取配置文件，如果有的话，会覆盖原来的配置
        }


        private string _appDir = string.Empty;
        /// <summary>
        /// 程序根目录
        /// </summary>
        public string RootDir
        {
            get
            {
                if (string.IsNullOrEmpty(_appDir))
                {
                    string fullName = Process.GetCurrentProcess().MainModule.FileName;
                    string dirName = Path.GetDirectoryName(fullName);
                }
                return _appDir;
            }
            private set
            {
                _appDir = value;
            }
        }

        public int HttpPort { get; private set; } = 12345;

        public int WebsocketPort { get; private set; } = 12346;


        public string StaticName { get; private set; } = "public";

        public string BaseRoute { get; private set; } = "/api/v1";
    }
}
