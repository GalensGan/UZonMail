using Microsoft.EntityFrameworkCore;
using UZonMailService.Models.SQL.Base;
using UZonMailService.Models.SQL.MultiTenant;

namespace UZonMailService.Models.SQL.Permission
{
    /// <summary>
    /// 权限码
    /// </summary>
    [Index(nameof(Code), IsUnique = true)]
    public class PermissionCode : SqlId
    {
        /// <summary>
        /// 权限码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
    }
}
