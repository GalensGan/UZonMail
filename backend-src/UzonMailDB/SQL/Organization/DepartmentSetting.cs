using UZonMail.DB.SQL.Base;
using UZonMail.DB.SQL.Organization;

namespace UZonMail.DB.SQL.Organization
{
    /// <summary>
    /// 部门元数据
    /// </summary>
    public class DepartmentSetting : SqlId
    {
        /// <summary>
        /// 部门 Id
        /// </summary>
        public long DepartmentId { get; set; }

        /// <summary>
        /// 子用户的策略
        /// </summary>
        public SubUserStrategy SubUserStrategy { get; set; }

        /// <summary>
        /// 从另一个对象中更新
        /// </summary>
        /// <param name="departmentSetting"></param>
        public void UpdateFrom(DepartmentSetting departmentSetting)
        {
            SubUserStrategy = departmentSetting.SubUserStrategy;
        }
    }
}
