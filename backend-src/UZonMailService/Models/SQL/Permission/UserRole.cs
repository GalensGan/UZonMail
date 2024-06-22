using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.UserInfos;

namespace UZonMailService.Models.SqlLite.Permission
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserRole : SqlId
    {
        /// <summary>
        /// 用户的 Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 权限角色的 Id
        /// </summary>
        public int RoleId { get; set; }

        #region 导航属性
        public User User { get; set; }
        public Role Role { get; set; }
        #endregion
    }
}
