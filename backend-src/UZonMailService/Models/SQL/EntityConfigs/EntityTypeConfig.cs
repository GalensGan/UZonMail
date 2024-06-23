using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace UZonMailService.Models.SQL.EntityConfigs
{
    // 指定 outbox 外键
    internal class EntityTypeConfig : IEntityTypeConfig
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            // 取消级联删除
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            // 对所有的实体配置 json 转换
            modelBuilder.AddJsonFields();
            // 应用配置，参考：https://learn.microsoft.com/zh-cn/ef/core/modeling/#applying-all-configurations-in-an-assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
