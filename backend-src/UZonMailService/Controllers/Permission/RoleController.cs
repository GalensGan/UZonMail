using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Permission;
using UZonMailService.Services.Settings;
using UZonMailService.Utils.ASPNETCore.PagingQuery;

namespace UZonMailService.Controllers.Permission
{
    [Route("api/v1/permission/[controller]")]
    public class RoleController(TokenService tokenService,SqlContext db) : ControllerBaseV1
    {
        /// <summary>
        /// 获取权限码数量
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
        /// 获取权限码数据
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
                .Include(x=>x.PermissionCodes)
                .ToListAsync();
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 批量添加权限码
        /// </summary>
        /// <param name="permissionCodes"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<ResponseResult<List<PermissionCode>>> CreateRoles([FromBody] List<Role> permissionCodes)
        {
            throw new Exception();
        }
    }
}
