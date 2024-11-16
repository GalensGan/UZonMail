using log4net;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UZonMail.DB.SQL.Settings;
using UZonMail.DB.SQL;
using System.Collections.Concurrent;

namespace UZonMail.Managers.Cache
{
    /// <summary>
    /// 缓存管理器
    /// </summary>
    public class CacheManager
    {
        private readonly static ILog _logger = LogManager.GetLogger(typeof(CacheManager));
        private static readonly ConcurrentDictionary<string, ICache> _settingsDic = [];

        /// <summary>
        /// 添加设置
        /// </summary>
        /// <param name="setting"></param>
        public static void AddCacheReader(ICache setting)
        {
            _settingsDic.TryAdd(setting.SettingKey, setting);
        }

        /// <summary>
        /// 获取完整的 Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetFullKey<T>(string key) where T : ICache, new()
        {
            return $"{typeof(T).FullName}:{key}";
        }

        /// <summary>
        /// 更新设置
        /// </summary>
        /// <param name="objectIdKey"></param>
        /// <returns></returns>
        public static bool UpdateCache<T>(string objectIdKey) where T : ICache, new()
        {
            var fullKey = GetFullKey<T>(objectIdKey);

            // 移除缓存数据
            if (!_settingsDic.TryGetValue(fullKey, out var value)) return false;
            value.NeedUpdate();
            return true;
        }

        /// <summary>
        /// 获取用户设置读取器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="objectIdKey">建议使用 objectId</param>
        /// <returns></returns>
        public static async Task<T?> GetCache<T>(SqlContext db, string objectIdKey) where T : ICache, new()
        {
            var fullKey = GetFullKey<T>(objectIdKey);

            if (!_settingsDic.TryGetValue(fullKey, out var value))
            {
                value = new T();
                _settingsDic.TryAdd(fullKey, value);
            }

            await value.Initialize(db, objectIdKey);
            return (T)value;
        }
    }
}
