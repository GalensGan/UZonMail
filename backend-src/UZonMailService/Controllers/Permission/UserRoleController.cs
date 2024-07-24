using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UZonMail.Utils.Web.Extensions;
using UZonMail.Utils.Web.ResponseModel;
using UZonMailService.UzonMailDB.SQL;
using UZonMailService.UzonMailDB.SQL.Permission;
using UZonMailService.UzonMailDB.Validators;
using UZonMailService.Services.Permission;
using UZonMailService.Services.Settings;
using UZonMailService.Utils.ASPNETCore.PagingQuery;
using UZonMailService.Utils.Database;
using UZonMailService.Utils.Extensions;

namespace UZonMailService.Controllers.Permission
{
    public class UserRoleController(SqlContext db, PermissionService permission) : PermissionControllerBase
    {
        /// <summary>
        /// 创建或者更新角色
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<ResponseResult<UserRole>> UpsertUserRole([FromBody] UserRole userRole)
        {
            var validator = new UserRoleValidator();
            var vdResult = validator.Validate(userRole);
            if (!vdResult.IsValid)
            {
                return vdResult.ToErrorResponse<UserRole>();
            }

            // 查找 更新 roles
            var roleIds = userRole.Roles.Select(x => x.Id);
            userRole.Roles = await db.Roles.Where(x => roleIds.Contains(x.Id)).ToListAsync();
            if (userRole.Id > 0)
            {
                var existOne = await db.UserRoles.Where(x => x.Id == userRole.Id)
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync();
                existOne.UserId = userRole.UserId;
                existOne.Roles.SetList(userRole.Roles);
                userRole = existOne;
            }
            else
            {
                db.UserRoles.Add(userRole);
            }

            await db.SaveChangesAsync();
            return userRole.ToSuccessResponse();
        }

        /// <summary>
        /// 获取用户角色数量
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("filtered-count")]
        public async Task<ResponseResult<int>> GetRolesCount(string filter)
        {
            var dbSet = db.UserRoles.AsNoTracking();
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.User.UserName.Contains(filter) || x.User.UserName.Contains(filter));
            }
            int count = await dbSet.CountAsync();
            return count.ToSuccessResponse();
        }

        /// <summary>
        /// 获取角色数据
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("filtered-data")]
        public async Task<ResponseResult<List<UserRole>>> GetRolesData(string filter, [FromBody] Pagination pagination)
        {
            var dbSet = db.UserRoles.AsNoTracking();
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.User.UserName.Contains(filter) || x.User.UserName.Contains(filter));
            }
            var results = await dbSet.Page(pagination)
                .Include(x => x.User)
                .Include(x => x.Roles)
                .ToListAsync();

            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 删除用户角色
        /// </summary>
        /// <param name="userRoleId"></param>
        /// <returns></returns>
        [HttpDelete("{userRoleId:long}")]
        public async Task<ResponseResult<bool>> DeleteUserRole(long userRoleId)
        {
            // 先移除关联的角色
            var userRole = await db.UserRoles.Where(x => x.Id == userRoleId)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync();
            if (userRole == null) return false.ToErrorResponse("未找到对应的用户角色");
            userRole.Roles.Clear();

            // 删除本身
            db.UserRoles.Remove(userRole);
            await db.SaveChangesAsync();

            // 更新权限缓存
            var permissionCodesDic = await permission.UpdateUserPermissionsCache([userRole.UserId]);
            // 通知权限更新
            await permission.NotifyPermissionUpdate(permissionCodesDic);

            // 返回结果
            return true.ToSuccessResponse();
        }
    }
}
