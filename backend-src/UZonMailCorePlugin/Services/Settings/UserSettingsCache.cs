using log4net;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
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
        private static async Task<UserSetting> GetSystemSettings(SqlContext db)
        {
            if (_systemUserId < 0)
            {
                // 获取系统用户 ID
                var systemUser = await db.Users.Where(x => x.IsSystemUser).Select(x => new { x.Id }).FirstOrDefaultAsync();
                if (systemUser == null)
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
        private static async Task<UserSetting> GetUserSettings(SqlContext db, long userId)
        {
            //string cacheKey = $"UserSettings:{userId}";
            //// 从缓存中获取设置
            //var cacheValue = await CacheService.Instance.GetAsync<UserSetting>(cacheKey);
            //if (cacheValue != null)
            //{
            //    return cacheValue;
            //}

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
            //await CacheService.Instance.SetAsync(cacheKey, settings);
            return settings;
        }

        /// <summary>
        /// 获取部门的设置
        /// </summary>
        /// <param name="db"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        private static async Task<UserSetting> GetDepartmentSetting(SqlContext db, long departmentId)
        {
            //string cacheKey = $"DepartmentSettings:{departmentId}";
            //// 从缓存中获取设置
            //var cacheValue = await CacheService.Instance.GetAsync<UserSetting>(cacheKey);
            //if (cacheValue != null)
            //{
            //    return cacheValue;
            //}

            var settings = await db.UserSettings.Where(x => x.DepartmentId == departmentId).FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new UserSetting
                {
                    DepartmentId = departmentId
                };
                db.UserSettings.Add(settings);
                await db.SaveChangesAsync();
            }

            // 获取缓存设置
            //await CacheService.Instance.SetAsync(cacheKey, settings);
            return settings;
        }

        /// <summary>
        /// 将用户设置更新到缓存
        /// </summary>
        /// <param name="userSetting"></param>
        /// <returns></returns>
        public static bool UpdateUserSettings(long userId)
        {
            // 移除缓存数据
            _userSettingsDic.TryRemove(userId, out _);
            return true;
        }

        private static ConcurrentDictionary<long, UserSettingsReader> _userSettingsDic = [];

        /// <summary>
        /// 获取用户设置读取器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<UserSettingsReader> GetUserSettingsReader(SqlContext db, long userId)
        {
            if (_userSettingsDic.TryGetValue(userId, out var value)) return value;

            var systemSetting = await GetSystemSettings(db);
            var userSetting = await GetUserSettings(db, userId);
            List<UserSetting> settings = [systemSetting, userSetting];
            var user = await db.Users.Where(x => x.Id == userId).FirstAsync();
            // 如果是子账户
            if (user.Type == UserType.SubUser)
            {
                // 查找部门设置
                var departmentMeta = await db.DepartmentSettings.Where(x => x.DepartmentId == user.DepartmentId).FirstOrDefaultAsync();
                if (departmentMeta != null && departmentMeta.SubUserStrategy == SubUserStrategy.FollowMaster)
                {
                    // 查找部门设置
                    var departmentSetting = await GetDepartmentSetting(db, user.DepartmentId);
                    settings.Add(departmentSetting);
                }
            }
            var setting = new UserSettingsReader(settings);
            _userSettingsDic.TryAdd(userId, setting);
            return setting;
        }
    }
}
