using Microsoft.EntityFrameworkCore;
using UZonMailService.Utils.Database;

namespace UZonMailService.Models.SQL.Updater.Updaters
{
    /// <summary>
    /// 更新用户的部门信息
    /// </summary>
    public class UserDepartmentInfoUpdater : IDataUpdater
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public Version Version => new(1, 0, 0, 0);

        /// <summary>
        /// 开始更新数据
        /// </summary>
        /// <returns></returns>
        public async Task Update(SqlContext db)
        {
            // 开始更新
            var systemDpIds = new List<long>() { 1, 2 };
            var systemDepartments = await db.Departments.Where(x => x.IsSystem && systemDpIds.Contains(x.Id))
                .ToListAsync();
            if (systemDepartments.Count != 2)
                throw new ApplicationException("系统部门数据不完整");

            // 开始更新所有用户
            await db.Users.UpdateAsync(x => true, x => x.SetProperty(y => y.OrganizationId, 1)
                .SetProperty(y => y.DepartmentId, 2));
        }
    }
}
