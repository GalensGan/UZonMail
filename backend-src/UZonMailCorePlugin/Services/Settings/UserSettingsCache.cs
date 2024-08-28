using log4net;
using Microsoft.EntityFrameworkCore;
using UZonMail.Core.Cache;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Organization;
using UZonMail.DB.SQL.Settings;

namespace UZonMail.Core.Services.Settings
{
    /// <summary>
    /// 用户设置缓存
    /// 传入的值一定不能为 null
    /// </summary>
    public class UserSettingsCache
    {
        private static ILog _logger = LogManager.GetLogger(typeof(UserSettingsCache));
        private static int _systemUserId = -1;
        /// <summary>
        /// 获取系统的设置
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static async Task<UserSetting> GetSystemSettings(SqlContext db)
        {
            if (_systemUserId < 0)
            {
                // 获取系统用户 ID
                var systemUser = await db.Users.Where(x => x.IsSystemUser).Select(x => new { x.Id }).FirstOrDefaultAsync();
                if(systemUser==null)
                {
                    _logger.Error("系统用户不存在");
                    _systemUserId = 0;
                }
            }

            if (_systemUserId < 1) return new UserSetting();            
            return await GetUserSettings(db, _systemUserId);
        }

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
            if (userSetting == null)
                return false;

            string cacheKey = $"UserSettings:{userSetting.UserId}";
            // 更新缓存
            return await CacheService.Instance.SetAsync(cacheKey, userSetting);
        }

        /// <summary>
        /// 获取用户设置读取器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<UserSettingsReader> GetUserSettingsReader(SqlContext db, long userId)
        {
            var systemSetting = await GetSystemSettings(db);
            var userSetting = await GetUserSettings(db, userId);

            return new UserSettingsReader(
            [
                systemSetting,
                userSetting,
            ]);
        }
    }
}
