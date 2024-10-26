﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UZonMail.Core.Config;
using UZonMail.Core.Config.SubConfigs;
using UZonMail.Core.Database.Updater;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Files;
using UZonMail.DB.SQL.Organization;
using UZonMail.DB.SQL.Permission;
using UZonMail.Utils.Extensions;

namespace UZonMail.Core.Database.SQL.Updaters
{
    public class InitSql : IDataUpdater
    {
        public Version Version => new("0.1.0.0");

        public async Task Update(SqlContext db, IConfiguration config)
        {
            // 初始化权限
            await InitPermission(db);

            // 初始化存储库
            await InitFileStorage(db, config);

            // 初始化用户
            await InitUser(db, config);
        }

        /// <summary>
        /// 初始化用户
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private async Task InitUser(SqlContext db, IConfiguration config)
        {
            // 判断是否有默认组织和部门
            var systemDepartments = await db.Departments.Where(x => x.IsSystem).ToListAsync();
            string systemOrgName = Department.SystemOrganizationName;
            var systemOrganization = systemDepartments.FirstOrDefault(x => x.Name == systemOrgName);
            if (systemOrganization == null)
            {
                systemOrganization = new Department
                {
                    Id = 1,
                    Name = systemOrgName,
                    Description = "系统组织",
                    FullPath = systemOrgName,
                    IsSystem = true,
                    IsHidden = true,
                    Type = DepartmentType.Organization
                };
                db.Add(systemOrganization);
            }
            // 判断是否有系统级部门
            string systemDpName = Department.SystemDepartmentName;
            var systemDepartment = systemDepartments.FirstOrDefault(x => x.Name == systemDpName && x.ParentId == systemOrganization.Id);
            if (systemDepartment == null)
            {
                systemDepartment = new Department
                {
                    Id = 2,
                    Name = systemDpName,
                    Description = "系统部门",
                    FullPath = $"{systemOrgName}/{systemDpName}",
                    ParentId = systemOrganization.Id,
                    IsSystem = true,
                    IsHidden = true,
                    Type = DepartmentType.Department,
                };
                db.Add(systemDepartment);
            }
            await db.SaveChangesAsync();

            // 设置系统用户，用于挂载系统相关的数据
            var systemUser = await db.Users.FirstOrDefaultAsync(x => x.IsSuperAdmin);
            if (systemUser == null)
            {
                systemUser = new User
                {
                    OrganizationId = systemOrganization.Id,
                    DepartmentId = systemDepartment.Id,
                    UserId = User.SystemUserId,
                    UserName = "系统用户",
                    // 系统用户无法登陆
                    Password = "system1234",
                    IsSystsemUser = true,
                    IsHidden = true
                };
                db.Add(systemUser);
            }
            // 保存系统用户
            await db.SaveChangesAsync();

            // 添加默认组织
            var defaultOrganization = await db.Departments.FirstOrDefaultAsync(x => x.Type == DepartmentType.Organization && x.Name == Department.DefaultOrganizationName);
            // 添加默认组织
            if (defaultOrganization == null)
            {
                defaultOrganization = new Department
                {
                    Id = 3,
                    Name = Department.DefaultOrganizationName,
                    Description = "默认组织",
                    FullPath = Department.DefaultDepartmentName,
                    IsHidden = false,
                    Type = DepartmentType.Organization
                };
                db.Add(defaultOrganization);
            }

            // 添加默认部门
            var defaultDepartment = await db.Departments.FirstOrDefaultAsync(x => x.Type == DepartmentType.Department && x.Name == Department.DefaultDepartmentName);
            if (defaultDepartment == null)
            {
                defaultDepartment = new Department
                {
                    Id = 4,
                    Name = Department.DefaultDepartmentName,
                    Description = "默认部门",
                    FullPath = $"{Department.DefaultOrganizationName}/{Department.DefaultDepartmentName}",
                    ParentId = defaultOrganization.Id,
                    IsHidden = false,
                    Type = DepartmentType.Department
                };
                db.Add(defaultDepartment);
            }
            await db.SaveChangesAsync();

            // 添加超管
            var adminUser = await db.Users.FirstOrDefaultAsync(x => x.IsSuperAdmin);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    OrganizationId = defaultOrganization.Id,
                    DepartmentId = defaultDepartment.Id,
                    UserId = "admin",
                    UserName = "admin",
                    // 密码是进行了 Sha256 二次加密
                    Password = "admin1234".Sha256(1),
                    IsSuperAdmin = true,
                    IsHidden = true,
                };

                var userConfig = new UserConfig();
                config.GetSection("User")?.Bind(userConfig);
                // 从配置中读取超管的信息
                if (userConfig.AdminUser!=null)
                {
                    var adminUserConfig = userConfig.AdminUser;
                    if (!string.IsNullOrEmpty(adminUserConfig.UserId)) adminUser.UserId = adminUserConfig.UserId;
                    if (!string.IsNullOrEmpty(adminUserConfig.Password)) adminUser.Password = adminUserConfig.Password.Sha256(1);
                    if (!string.IsNullOrEmpty(adminUserConfig.Avatar)) adminUser.Avatar = adminUserConfig.Avatar;
                }

                db.Add(adminUser);
            }
            await db.SaveChangesAsync();

            // 添加组织管理员角色
            var orgAdminRole = await db.Roles.FirstOrDefaultAsync(x => x.Name == Role.OrganizationAdminRoleName);
            if (orgAdminRole == null)
            {
                var permissionCode = await db.PermissionCodes.FirstOrDefaultAsync(x => x.Code == PermissionCode.OrganizationPermissionCode);
                orgAdminRole = new Role
                {
                    Name = Role.OrganizationAdminRoleName,
                    Description = "拥有管理所在组织所有功能的权限",
                    OrganizationId = defaultOrganization.Id
                };
                orgAdminRole.PermissionCodes.Add(permissionCode);
                db.Add(orgAdminRole);
            }
            await db.SaveChangesAsync();

            // 为超管添加组织管理员角色
            var userRole = new UserRoles
            {
                UserId = adminUser.Id,
                Roles = [orgAdminRole]
            };
            db.Add(userRole);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// 初始化用户角色
        /// 1. 添加默认功能码,包含管理员
        /// 2. 生成一个管理员角色和一个普通用户角色
        /// 3. 为角色赋予默认功能码
        /// </summary>
        private async Task InitPermission(SqlContext db)
        {
            // 检查是否已经有数据
            if (db.Users.Any())
            {
                return;   // 如果已经有数据，直接返回
            }

            var permissionCode = new PermissionCode() { Code = PermissionCode.OrganizationPermissionCode, Description = "拥有管理所在组织所有功能的权限" };
            db.PermissionCodes.Add(permissionCode);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// 初始化存储库
        /// </summary>
        private async Task InitFileStorage(SqlContext db, IConfiguration config)
        {
            bool existDefaultFileBucket = await db.FileBuckets.AnyAsync(x => x.IsDefault);
            if (existDefaultFileBucket) return;

            var fileStorage = new FileStorageConfig();
            config.GetSection("FileStorage")?.Bind(fileStorage);

            // 新建
            var defaultBucket = new FileBucket
            {
                BucketName = "default",
                Description = "默认存储桶",
                RootDir = fileStorage.DefaultRootDir,
                IsDefault = true
            };

            // 创建目录
            Directory.CreateDirectory(defaultBucket.RootDir);
            db.FileBuckets.Add(defaultBucket);
        }
    }
}
