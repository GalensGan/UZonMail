using log4net;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Organization;
using UZonMail.DB.SQL.Settings;

namespace UZonMail.Core.Services.Settings
{
    /// <summary>
    /// 用户设置缓存
    /// 传入的值一定不能为 null
    /// </summary>
    public class SettingsCache
    {
        private static ILog _logger = LogManager.GetLogger(typeof(SettingsCache));
        private static List<SettingsReader> _settingReaders = [];

        /// <summary>
        /// 更新设置
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static bool UpdateSettings(long organizationId)
        {
            // 移除缓存数据
            _settingReaders = _settingReaders.Where(x => !x.IsMatch(organizationId)).ToList();
            return true;
        }

        /// <summary>
        /// 获取用户设置读取器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<SettingsReader> GetSettingsReader(SqlContext db, long userId)
        {
            var settingReader = _settingReaders.FirstOrDefault(x => x.IsMatch(userId));
            if (settingReader != null) return settingReader;

            // 若不存在，则创建
            var user = await db.Users.AsNoTracking().Where(x => x.Id == userId).FirstOrDefaultAsync() ?? throw new NullReferenceException($"User {userId} not found");
            var orgSetting = await db.OrganizationSettings.FirstOrDefaultAsync(x => x.OrganizationId == user.OrganizationId) ?? new OrganizationSetting()
            {
                OrganizationId = user.OrganizationId
            };
            settingReader = new SettingsReader(user.OrganizationId);
            settingReader.AddSetting(orgSetting);
            _settingReaders.Add(settingReader);
            return settingReader;
        }
    }
}
