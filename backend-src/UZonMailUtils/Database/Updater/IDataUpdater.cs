using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
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
        Task Update(SqlContext db, IConfiguration config);
    }
}
