using UZonMail.DB.SQL.Base;
using UZonMail.DB.SQL.Organization;

namespace UZonMail.DB.SQL.Organization
{
    /// <summary>
    /// 部门元数据
    /// </summary>
    public class DepartmentMeta : SqlId
    {
        /// <summary>
        /// 部门 Id
        /// </summary>
        public long DepartmentId { get; set; }

        /// <summary>
        /// 子用户的策略
        /// </summary>
        public SubUserStrategy SubUserStrategy { get; set; }


    }
}
