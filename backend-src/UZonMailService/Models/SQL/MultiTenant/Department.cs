﻿using UZonMailService.Models.SQL.Base;

namespace UZonMailService.Models.SQL.MultiTenant
{
    /// <summary>
    /// 部门表
    /// </summary>
    public class Department : SqlId
    {
        /// <summary>
        /// 部门名称-描述也是部门
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 父组部门 id
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 完整路径
        /// 用户快速查找部门下面的所有子部门
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 部门类型
        /// 组织中不能有用户
        /// </summary>
        public DepartmentType Type { get; set; }

        /// <summary>
        /// 是否是系统级部门
        /// 由初始化创建的部门属于系统级
        /// </summary>
        public bool IsSystem { get; set; }

        #region 静态方法
        public static string GetSystemOrganizationName()
        {
            return "SystemOrganization";
        }
        public static string GetSystemDepartmentName()
        {
            return "SystemDepartment";
        }
        #endregion
    }
}
