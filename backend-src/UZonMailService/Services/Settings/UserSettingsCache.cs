using Microsoft.EntityFrameworkCore;
using UZonMailService.Cache;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.MultiTenant;
using UZonMailService.Models.SQL.Settings;

namespace UZonMailService.Services.Settings
{
    /// <summary>
    /// 用户设置缓存
    /// 传入的值一定不能为 null
    /// </summary>
    public class UserSettingsCache
    {
        /// <summary>
        /// 创建用户设置
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<UserSetting> GetUserSettings(SqlContext db, long userId)
        {
            string cacheKey = $"UserSettings:{userId}";
            // 从缓存中获取设置
            var cacheValue = await CacheService.Instance.GetAsync<UserSetting>(cacheKey);
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var settings = await db.UserSettings.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new UserSetting
                {
                    UserId = userId
                };
                db.UserSettings.Add(settings);
                await db.SaveChangesAsync();
            }                

            // 获取缓存设置
            await CacheService.Instance.SetAsync(cacheKey, settings);
            return settings;
        }

        /// <summary>
        /// 将用户设置更新到缓存
        /// </summary>
        /// <param name="userSetting"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateUserSettings(UserSetting? userSetting)
        {
            if(userSetting == null)
                return false;

            string cacheKey = $"UserSettings:{userSetting.UserId}";
            // 更新缓存
           return await CacheService.Instance.SetAsync(cacheKey, userSetting);
        }
    }
}
