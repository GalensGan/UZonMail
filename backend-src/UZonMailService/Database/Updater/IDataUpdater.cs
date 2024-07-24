using Microsoft.EntityFrameworkCore;
using UZonMailService.Utils.Database;
using UZonMailService.UzonMailDB.SQL;

namespace UZonMailService.Database.Updater
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
