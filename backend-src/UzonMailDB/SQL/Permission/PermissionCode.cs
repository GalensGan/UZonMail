using Microsoft.EntityFrameworkCore;
using UZonMail.DB.SQL.Base;
using UZonMail.DB.SQL.MultiTenant;

namespace UZonMail.DB.SQL.Permission
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

        /// <summary>
        /// 关联的角色
        /// </summary>
        public List<Role> Roles { get; set; }
    }
}
