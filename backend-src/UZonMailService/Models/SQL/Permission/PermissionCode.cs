using UZonMailService.Models.SqlLite.Base;

namespace UZonMailService.Models.SqlLite.Permission
{
    /// <summary>
    /// 权限码
    /// </summary>
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
