using Microsoft.EntityFrameworkCore;
using UZonMailService.Utils.Database;

namespace UZonMailService.Models.SQL.Updater
{
    public interface IDataUpdater
    {
        /// <summary>
        /// 版本号
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// 开始更新数据
        /// </summary>
        /// <returns></returns>
        Task Update(SqlContext db);
    }
}
