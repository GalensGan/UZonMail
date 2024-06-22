using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UZonMailService.Models.SqlLite.Emails;

namespace UZonMailService.Models.SqlLite.EntityConfigs
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
