using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.Permission;
using UZonMailService.Services.Settings;
using UZonMailService.Utils.ASPNETCore.PagingQuery;

namespace UZonMailService.Controllers.Permission
{
    /// <summary>
    /// 权限码路由
    /// </summary>
    [Route("api/v1/permission/[controller]")]
    public class PermissionCodeController(TokenService tokenService, SqlContext db) : ControllerBaseV1
    {
        /// <summary>
        /// 获取权限码数量
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("filtered-count")]
        public async Task<ResponseResult<int>> GetPermissionCodesCount(string filter)
        {
            var dbSet = db.PermissionCodes.AsNoTracking();
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Code.Contains(filter) || x.Description.Contains(filter));
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
        public async Task<ResponseResult<List<PermissionCode>>> GetPermissionCodesData(string filter, [FromBody] Pagination pagination)
        {
            var dbSet = db.PermissionCodes.AsNoTracking();
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Code.Contains(filter) || x.Description.Contains(filter));
            }
            var results = await dbSet.Page(pagination).ToListAsync();
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 获取所有的权限码
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<ResponseResult<List<PermissionCode>>> GetAllPermissionCodes()
        {
            var results = await db.PermissionCodes.AsNoTracking().ToListAsync();
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 批量添加权限码
        /// </summary>
        /// <param name="permissionCodes"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut()]
        public async Task<ResponseResult<List<PermissionCode>>> UpdatePermissionCode([FromBody] List<PermissionCode> permissionCodes)
        {
            permissionCodes = permissionCodes.Where(x => !string.IsNullOrEmpty(x.Code)).ToList();
            var codes = permissionCodes.Select(x => x.Code).ToList();
            // 查找存在的权限码
            var existCodes = await db.PermissionCodes.Where(x => codes.Contains(x.Code)).ToListAsync();
            // 过滤掉已经存在的权限码
            foreach (var permissionCode in permissionCodes)
            {
                // 判断是否存在
                var existCode = existCodes.FirstOrDefault(x => x.Code == permissionCode.Code);
                if (existCode != null)
                {
                    existCode.Description = permissionCode.Description;
                    continue;
                }
                db.PermissionCodes.Add(permissionCode);
            }

            await db.SaveChangesAsync();

            return permissionCodes.Where(x => x.Id > 0).ToList()
                .ToSuccessResponse();
        }
    }
}
