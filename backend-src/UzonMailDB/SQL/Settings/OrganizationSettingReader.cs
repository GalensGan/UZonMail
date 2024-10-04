using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using UZonMail.DB.Extensions;
using UZonMail.DB.SQL.Organization;
using UZonMail.DB.SQL.Settings;
using UZonMail.DB.SQL.Unsubscribes;
using UZonMail.Managers.Cache;

namespace UZonMail.DB.SQL.Settings
{
    /// <summary>
    /// 后者的值会覆盖前者的值
    /// </summary>
    /// <param name="settings"></param>
    public class OrganizationSettingReader : OrganizationSetting, ICacheReader
    {
        #region 接口实现
        /// <summary>
        /// 键值
        /// </summary>
        public string? SettingKey { get; private set; }

        private bool _needUpdate = true;

        /// <summary>
        /// key 为 organizationId
        /// </summary>
        /// <param name="db"></param>
        /// <param name="organizationObjectId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task Initialize(SqlContext db, string organizationObjectId)
        {
            if (!_needUpdate) return;
            _needUpdate = false;

            if (string.IsNullOrEmpty(organizationObjectId)) throw new ArgumentException("organizationObjectId is null or empty");

            // 获取设置
            // key 为 organizationId
            SettingKey = CacheManager.GetFullKey<OrganizationSettingReader>(organizationObjectId);
            var organization = await db.Departments.AsNoTracking().FirstOrDefaultAsync(x => x.ObjectId == organizationObjectId);

            var setting = await db.OrganizationSettings.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == organization.Id);
            if (setting == null)
            {
                // 添加默认值
                setting = new OrganizationSetting();
                db.Add(setting);
                await db.SaveChangesAsync();
            }

            // 赋值
            this.CopyAllProperties(setting);
        }

        public void NeedUpdate()
        {
            _needUpdate = true;
        }
        #endregion
    }
}
