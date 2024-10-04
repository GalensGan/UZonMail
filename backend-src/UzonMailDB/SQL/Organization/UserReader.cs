using Microsoft.EntityFrameworkCore;
using UZonMail.DB.Extensions;
using UZonMail.Managers.Cache;

namespace UZonMail.DB.SQL.Organization
{
    public class UserReader: ICacheReader
    {
        public string? SettingKey { get; private set; }

        private bool _needUpdate = true;
        /// <summary>
        /// key 为 userId
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task Initialize(SqlContext db, string key)
        {
            if(!_needUpdate) return;
            _needUpdate = false;

            SettingKey = $"{GetType().FullName}_{key}";
            if (!long.TryParse(key, out var userId)) return;

            // 获取用户信息
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            var organization = await db.Departments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.OrganizationId);

            // 添加值
            UserId = user.Id;
            DepartmentId = user.DepartmentId;
            OrganizationId = user.OrganizationId;
            OrganizationObjectId = organization.ObjectId;
        }

        public void NeedUpdate()
        {
            _needUpdate = true;
        }

        public long UserId { get; private set; }
        public long DepartmentId { get; private set; }
        public long OrganizationId { get; private set; }
        public string OrganizationObjectId { get; private set; }
    }
}
