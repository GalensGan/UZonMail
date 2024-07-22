using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UZonMailService.Models.SQL.Base;
using UZonMailService.Models.SQL.MultiTenant;

namespace UZonMailService.Models.SQL.Permission
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserRole : SqlId
    {
        /// <summary>
        /// 用户的 Id
        /// </summary>
        public long UserId { get; set; }

        #region 导航属性
        public User User { get; set; }
        public List<Role> Roles { get; set; }
        #endregion
    }
}
