using Microsoft.Data.Sqlite;

namespace UZonMail.DB.SqLite
{
    public class SqLiteConnectionConfig
    {
        public bool Enable { get; set; } = false;
        public string DataSource { get; set; }
        public string Version { get; set; }
        public string Cache { get; set; }
        public string Mode { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// 数据库路径
        /// </summary>
        public string GetSqLiteFilePath()
        {
            if (!string.IsNullOrEmpty(DataSource))
            {
                // 只有为全路径时，才使用
                if (Path.IsPathRooted(DataSource))
                    return DataSource;
            }

            // 使用默认路径
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var subPath = "UZonMail\\uzon-mail.db";
            return Path.Join(localAppData, subPath);
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                var dataSource = GetSqLiteFilePath();
                SqliteConnectionStringBuilder builder = new()
                {
                    { "Data Source", dataSource },
                    { "Mode", Mode },
                    { "Cache", Cache },
                    { "Password", Password },
                    { "Version", Version }
                };

                return builder.ConnectionString;
            }
        }
    }
}
