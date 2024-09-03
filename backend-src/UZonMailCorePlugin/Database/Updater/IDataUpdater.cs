using Microsoft.EntityFrameworkCore;
using UZonMail.Core.Config;
using UZonMail.Core.Utils.Database;
using UZonMail.DB.SQL;

namespace UZonMail.Core.Database.Updater
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
        Task Update(SqlContext db,AppConfig config);
    }
}
