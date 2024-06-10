using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace UZonMailService.Models.SqlLite.EntityConfigs
{
    // 指定 outbox 外键
    internal class EntityTypeConfig : IEntityTypeConfig
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            // 取消级联删除
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientNoAction;
            }

            // 对所有的实体配置 json 转换
            modelBuilder.AddJsonFields();
            // 应用配置，参考：https://learn.microsoft.com/zh-cn/ef/core/modeling/#applying-all-configurations-in-an-assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 配置 json 映射
        /// 根据 JsonMapAttribute 判断是否需要设置 json 映射
        /// 若有，则设置 json 映射
        /// </summary>
        /// <param name="builder"></param>
        private static void ConfigureJsonMap(ModelBuilder modelBuilder)
        {
            
        }
    }
}
