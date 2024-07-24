using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UZonMailService.UzonMailDB.SQL.Emails;

namespace UZonMailService.UzonMailDB.SQL.EntityConfigs
{
    /// <summary>
    /// 配置实体类型
    /// </summary>
    public interface IEntityTypeConfig
    {
        /// <summary>
        /// 配置实体
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(ModelBuilder builder);
    }
}
