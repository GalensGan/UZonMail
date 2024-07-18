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
using UZonMailService.Utils.Extensions;

namespace UZonMailService.Controllers.Permission
{
    [Route("api/v1/permission/[controller]")]
    public class RoleController(TokenService tokenService, SqlContext db) : ControllerBaseV1
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
                dbSet = dbSet = dbSet.Where(x => x.Name.Contains(filter));
            }
            var results = await dbSet.Page(pagination)
                .Include(x => x.PermissionCodes)
                .ToListAsync();

            results.ForEach(x => x.PermissionCodeIds = x.PermissionCodes.Select(y => y.Id).ToList());
            return results.ToSuccessResponse();
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

            var permissionCodes = await db.PermissionCodes.AsNoTracking()
                .Where(x => role.PermissionCodeIds.Contains(x.Id))
                .ToListAsync();

            // 添加角色
            var existRole = await db.Roles.FirstOrDefaultAsync(x => x.Name == role.Name);
            if (existRole != null)
            {
                // 说明存在，更新
                existRole.Description = role.Description;
                existRole.PermissionCodes = permissionCodes;
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
    }
}
