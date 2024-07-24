
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UZonMailService.UzonMailDB.SQL;
using UZonMailService.UzonMailDB.SqLite;

namespace UZonMailService.UzonMailDB.SqLite
{
    public class SqLiteContext : SqlContext
    {
        private readonly SqLiteConnectionConfig _sqLiteConnectionConfig;
        public SqLiteContext(IConfiguration configuration)
        {
            // sqlLite
            _sqLiteConnectionConfig = new SqLiteConnectionConfig();
            configuration.GetSection("Database:SqLite").Bind(_sqLiteConnectionConfig);

            var sqlLiteFilePath = _sqLiteConnectionConfig.GetSqLiteFilePath();
            if (!string.IsNullOrEmpty(sqlLiteFilePath))
            {
                var directory = Path.GetDirectoryName(sqlLiteFilePath);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(_sqLiteConnectionConfig.ConnectionString);
        }
    }
}
