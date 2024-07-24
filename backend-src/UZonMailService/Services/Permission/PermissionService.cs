using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UZonMail.Utils.Web.Service;
using UZonMailService.Cache;
using UZonMailService.UzonMailDB.SQL;
using UZonMailService.UzonMailDB.SQL.MultiTenant;
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
        /// 生成权限缓存的 key
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetPermissionCacheKey(long userId) => $"permissions/{userId}";

        /// <summary>
        /// 更新用户的权限缓存
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns>返回权限码</returns>
        public async Task<Dictionary<long, List<string>>> UpdateUserPermissionsCache(List<long> userIds)
        {
            if (userIds.Count == 0) return [];

            var userRoles = await db.UserRoles.AsNoTracking()
                .Where(x => userIds.Contains(x.UserId))
                .Include(x => x.Roles)
                .ThenInclude(x => x.PermissionCodes)
                .GroupBy(x => x.UserId)
                .ToListAsync();

            Dictionary<long, List<string>> results = [];
            foreach (var item in userRoles)
            {
                var permissionCodes = item.SelectMany(x => x.Roles).SelectMany(x => x.PermissionCodes).Select(x => x.Code).Distinct().ToList();
                results.Add(item.Key, permissionCodes);
                // 更新缓存
                await cache.SetAsync(GetPermissionCacheKey(item.Key), permissionCodes);
            }
            return results;
        }

        /// <summary>
        /// 更新单个用户的权限缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<string>> UpdateUserPermissionsCache(long userId)
        {
            var results = await UpdateUserPermissionsCache([userId]);
            if (results.TryGetValue(userId, out var value)) return value;
            return [];
        }

        /// <summary>
        /// 获取用户权限码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetUserPermissionCodes(long userId)
        {
            var cacheValues = await cache.GetAsync<List<string>>(GetPermissionCacheKey(userId));
            // 更新缓存
            cacheValues ??= await UpdateUserPermissionsCache(userId);

            // 添加管理员权限码
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            // TODO: 根据是否授权是否返回管理员权限码
            //if (user != null && user.IsSuperAdmin)
            //    cacheValues.AddRange(["admin", "*"]);

            return cacheValues;
        }

        /// <summary>
        /// 通知用户权限更新
        /// </summary>
        /// <param name="userPermissionCodes"></param>
        /// <returns></returns>
        public async Task NotifyPermissionUpdate(Dictionary<long, List<string>> userPermissionCodes)
        {
            if (userPermissionCodes.Count == 0) return;

            foreach (var item in userPermissionCodes)
            {
                await hub.GetUserClient(item.Key).PermissionUpdated(item.Value);
            }
        }
    }
}
