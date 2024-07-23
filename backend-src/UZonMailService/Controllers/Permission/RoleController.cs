using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Permission;
using UZonMailService.Models.Validators;
using UZonMailService.Services.Settings;
using UZonMailService.Utils.ASPNETCore.PagingQuery;
using UZonMailService.Utils.Database;
using UZonMailService.Utils.Extensions;

namespace UZonMailService.Controllers.Permission
{
    public class RoleController(TokenService tokenService, SqlContext db) : PermissionControllerBase
    {
        /// <summary>
        /// 获取角色数量
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("filtered-count")]
        public async Task<ResponseResult<int>> GetRolesCount(string filter)
        {
            var userId = tokenService.GetUserDataId();
            var dbSet = db.Roles.AsNoTracking();
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Name.Contains(filter));
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
        public async Task<ResponseResult<List<Role>>> GetRolesData(string filter, [FromBody] Pagination pagination)
        {
            var userId = tokenService.GetUserDataId();
            var dbSet = db.Roles.AsNoTracking();
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Name.Contains(filter));
            }
            var results = await dbSet.Page(pagination)
                .Include(x => x.PermissionCodes)
                .ToListAsync();

            results.ForEach(x => x.PermissionCodeIds = x.PermissionCodes.Select(y => y.Id).ToList());
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 获取所有的角色
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ResponseResult<List<Role>>> GetAllRoles()
        {
            var roles = await db.Roles.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();
            return roles.ToSuccessResponse();
        }

        /// <summary>
        /// 添加或者更新角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost("upsert")]
        public async Task<ResponseResult<Role>> UpsertRole([FromBody] Role role)
        {
            var roleValidator = new RoleValidator();
            var vdResult = roleValidator.Validate(role);
            if (!vdResult.IsValid) return vdResult.ToErrorResponse<Role>();

            var permissionCodes = await db.PermissionCodes
                .Where(x => role.PermissionCodeIds.Contains(x.Id))
                .ToListAsync();

            // 添加角色
            var existRole = await db.Roles.Where(x => x.Id == role.Id)
                .Include(x => x.PermissionCodes)
                .FirstOrDefaultAsync();
            if (existRole != null)
            {
                // 说明存在，更新
                existRole.Name = role.Name;
                existRole.Description = role.Description;
                // 更改权限码
                existRole.PermissionCodes.SetList(permissionCodes);
            }
            else
            {
                role.PermissionCodes = permissionCodes;
                db.Roles.Add(role);
                existRole = role;
            }
            await db.SaveChangesAsync();

            var result = new Role()
            {
                Id = existRole.Id,
                Name = existRole.Name,
                Description = existRole.Description,
                Icon = existRole.Icon,
                PermissionCodeIds = role.PermissionCodeIds
            };
            return result.ToSuccessResponse();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpDelete("{roleId:long}")]
        public async Task<ResponseResult<bool>> DeleteRole(long roleId)
        {
            // 删除角色

            // 删除用户与角色关联表

            // 重新计算受影响的用户的权限

            // 通知用户更新权限

            return true.ToSuccessResponse();
        }
    }
}
