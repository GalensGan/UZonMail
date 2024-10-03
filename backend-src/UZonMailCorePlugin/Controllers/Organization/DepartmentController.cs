using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.ResponseModel;
using UZonMail.Core.Services.Settings;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Organization;
using UZonMail.Utils.Web.ResponseModel;

namespace UZonMail.Core.Controllers.Organization
{
    public class DepartmentController(SqlContext db) : ControllerBaseV1
    {
        /// <summary>
        /// 获取部门设置
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet("{departmentId:long}")]
        public async Task<ResponseResult<DepartmentSetting>> GetDepartmentSetting(long departmentId)
        {
            var departmentSetting = await db.DepartmentSettings.FirstOrDefaultAsync(x => x.DepartmentId == departmentId);
            departmentSetting ??= new DepartmentSetting();

            return departmentSetting.ToSuccessResponse();
        }

        /// <summary>
        /// 更新部门设置
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{departmentId:long}")]
        public async Task<ResponseResult<bool>> UpdateDepartmentSettings(long departmentId, [FromBody] DepartmentSetting departmentSetting)
        {
            // 更新部门设置
            var existOne = await db.DepartmentSettings.FirstOrDefaultAsync(x => x.DepartmentId == departmentId);
            if (existOne == null) db.Add(departmentSetting);
            else
            {
                existOne.UpdateFrom(departmentSetting);
            }
            await db.SaveChangesAsync();

            // 更新部门中所有子用户的设置
            var departmentUserIds = await db.Users.Where(x => x.DepartmentId == departmentId && x.Type == UserType.SubUser).Select(x => x.Id).ToListAsync();
            departmentUserIds.ForEach(x => SettingsCache.UpdateSettings(x));

            return true.ToSuccessResponse();
        }
    }
}
