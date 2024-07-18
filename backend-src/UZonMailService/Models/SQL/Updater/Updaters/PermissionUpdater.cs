
using Microsoft.EntityFrameworkCore;
using UZonMailService.Models.SQL.Permission;
using UZonMailService.Utils.Database;

namespace UZonMailService.Models.SQL.Updater.Updaters
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
                new(){ Code = "PermissionCode1", Description = "PermissionCode1" },
            ];
        }
    }
}
