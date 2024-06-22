using UZonMailService.Models.SQL.Base;
using UZonMailService.Models.SQL.Permission;

namespace UZonMailService.Models.SQL.UserInfos
{
    /// <summary>
    /// 用户上下文
    /// </summary>
    public class User : SqlId
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// signalR 连接 id
        /// </summary>
        public string? ConnectionId { get; set; }       

        /// <summary>
        /// 禁止登录
        /// </summary>
        public bool ForbiddenToLogin { get; set; }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 是否是系统用户
        /// </summary>
        public bool IsSystemUser { get; set; }

        /// <summary>
        /// 用户角色
        /// 导航属性
        /// </summary>
        public List<UserRole> UserRoles { get; set; }
    }
}
