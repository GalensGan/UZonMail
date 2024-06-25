using UZonMailService.Models.SQL.Base;

namespace UZonMailService.Models.SQL.Permission
{
    /// <summary>
    /// Role-PermissionCode 关系
    /// </summary>
    public class RolePermissionCode : SqlId
    {
        public long RoleId { get; set; }

        public long PermissionCodeId { get; set; }
    }
}
