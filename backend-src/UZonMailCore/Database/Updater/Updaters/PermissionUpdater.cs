
using Microsoft.EntityFrameworkCore;
using UZonMail.DB.SQL.Permission;
using UZonMail.Core.Utils.Database;
using UZonMail.Core.Database.Updater;
using UZonMail.DB.SQL;

namespace UZonMail.Core.Database.Updater.Updaters
{
    /// <summary>
    /// 权限模块数据初始化
    /// </summary>
    public class PermissionUpdater : IDataUpdater
    {
        public Version Version => new Version(1, 0, 1, 0);

        public async Task Update(SqlContext db)
        {
            // 添加功能码
            List<PermissionCode> permissionCodes =
            [
                new() { Code = "PermissionCode1", Description = "PermissionCode1" },
            ];
        }
    }
}
