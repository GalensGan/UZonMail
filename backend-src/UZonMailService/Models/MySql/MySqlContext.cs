using Microsoft.EntityFrameworkCore;
using UZonMailService.Models.SQL;

namespace UZonMailService.Models.MySql
{
    public class MySqlContext : SqlContext
    {
        private readonly MySqlConnectionConfig _mysqlConnectionConfig;
        public MySqlContext(IConfiguration configuration, ILogger<SqlContext> logger)
        {
            _mysqlConnectionConfig = new();
            configuration.GetSection("Database:MySql").Bind(_mysqlConnectionConfig);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(_mysqlConnectionConfig.ConnectionString, new MySqlServerVersion(_mysqlConnectionConfig.MysqlVersion));
        }
    }
}
