using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Reflection.Metadata;
using UZonMailService.Models.MySql;
using UZonMailService.Models.SQL.Base;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.EmailSending;
using UZonMailService.Models.SQL.EntityConfigs;
using UZonMailService.Models.SQL.Settings;
using UZonMailService.Models.SQL.Templates;
using UZonMailService.Models.SQL.Tests;

namespace UZonMailService.Models.SQL
{
    /// <summary>
    /// Sql 上下文
    /// 参考：https://learn.microsoft.com/zh-cn/ef/core/modeling/relationships/conventions
    /// </summary>
    public class SqlContext : DbContext
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SqlContext));

        #region 初始化
        /// <summary>
        /// 配置数据库
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new EntityTypeConfig().Configure(modelBuilder);
        }
        #endregion

        #region 数据表定义
        public DbSet<MultiTenant.Department> Departments { get; set; }
        public DbSet<MultiTenant.User> Users { get; set; }
        public DbSet<Permission.PermissionCode> PermissionCodes { get; set; }
        public DbSet<Permission.Role> Roles { get; set; }
        public DbSet<Permission.RolePermissionCode> RolePermissionCodes { get; set; }
        public DbSet<Permission.UserRole> UserRoles { get; set; }

        public DbSet<Files.FileBucket> FileBuckets { get; set; }
        public DbSet<Files.FileObject> FileObjects { get; set; }
        public DbSet<Files.FileUsage> FileUsages { get; set; }
        public DbSet<Files.FileReader> FileReaders { get; set; }

        public DbSet<EmailGroup> EmailGroups { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Inbox> Inboxes { get; set; }
        public DbSet<Outbox> Outboxes { get; set; }

        public DbSet<SendingGroup> SendingGroups { get; set; }
        public DbSet<SendingItem> SendingItems { get; set; }
        public DbSet<SendingItemInbox> SendingItemInboxes { get; set; }

        public DbSet<UserProxy> UserProxies { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        #endregion

        #region 测试
        //public DbSet<Post> Posts { get; set; }
        //public DbSet<Tag> Tags { get; set; }
        #endregion

        #region 通用方法
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<T> RunTransaction<T>(Func<SqlContext, Task<T>> func)
        {
            using var transaction = await Database.BeginTransactionAsync();
            try
            {
                // 执行一些数据库操作
                var result = await func(this);
                // 如果所有操作都成功，那么提交事务
                await transaction.CommitAsync();
                return result;
            }
            catch (Exception e)
            {
                // 如果有任何操作失败，那么回滚事务
                await transaction.RollbackAsync();

                // 向外抛出异常
                throw;
            }
        }
        #endregion
    }
}
