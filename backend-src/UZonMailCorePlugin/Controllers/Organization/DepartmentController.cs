using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Organization;
using UZonMail.Utils.Web.Extensions;
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
            return true.ToSuccessResponse();
        }
    }
}
