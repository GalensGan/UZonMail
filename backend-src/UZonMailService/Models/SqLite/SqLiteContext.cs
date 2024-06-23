
using Microsoft.EntityFrameworkCore;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SqLite;

namespace UZonMailService.Models.SqLite
{
    public class SqLiteContext : SqlContext
    {
        private readonly SqLiteConnectionConfig _sqLiteConnectionConfig;
        public SqLiteContext(IConfiguration configuration, ILogger<SqlContext> logger)
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
