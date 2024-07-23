using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.Service;
using UZonMailService.Cache;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.MultiTenant;
using UZonMailService.SignalRHubs;
using UZonMailService.SignalRHubs.Extensions;

namespace UZonMailService.Services.Permission
{
    /// <summary>
    /// 权限服务
    /// </summary>
    public class PermissionService(SqlContext db, CacheService cache, IHubContext<UzonMailHub, IUzonMailClient> hub) : IScopedService
    {
        /// <summary>
        /// 更新用户的权限缓存
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns>返回权限码</returns>
        public async Task<Dictionary<long,List<string>>> UpdateUserPermissionsCache(List<long> userIds)
        {
            if (userIds.Count == 0) return [];

            var userRoles = await db.UserRoles.AsNoTracking()
                .Where(x => userIds.Contains(x.UserId))
                .Include(x => x.Roles)
                .ThenInclude(x => x.PermissionCodes)
                .GroupBy(x=>x.UserId)
                .ToListAsync();
            
            Dictionary<long, List<string>> results = [];
            foreach (var item in userRoles)
            {
                var permissionCodes = item.SelectMany(x=>x.Roles).SelectMany(x=>x.PermissionCodes).Select(x => x.Code).Distinct().ToList();
                results.Add(item.Key, permissionCodes);
                // 更新缓存
                await cache.SetAsync($"permissions/{item.Key}", permissionCodes);
            }
            return results;
        }

        /// <summary>
        /// 通知用户权限更新
        /// </summary>
        /// <param name="userPermissionCodes"></param>
        /// <returns></returns>
        public async Task NotifyPermissionUpdate(Dictionary<long,List<string>> userPermissionCodes)
        {
            if(userPermissionCodes.Count == 0) return;

            foreach (var item in userPermissionCodes)
            {
                await hub.GetUserClient(item.Key).PermissionUpdated(item.Value);
            }
        }
    }
}
