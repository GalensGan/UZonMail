using Microsoft.EntityFrameworkCore;
using UZonMail.Core.Config;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Files;
using UZonMail.DB.SQL.Organization;
using UZonMail.Utils.Extensions;

namespace UZonMail.Core.Database.Updater.Updaters
{
    public class InitSql : IDataUpdater
    {
        public Version Version => new Version("0.0.0.1");

        public async Task Update(SqlContext db, AppConfig config)
        {
            await InitUser(db, config);
            await InitPermission(db);
            await InitFileStorage(db, config);
        }

        /// <summary>
        /// 初始化用户
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private async Task InitUser(SqlContext db, AppConfig config)
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

            // 设置系统用户，用于挂载系统相关的数据
            var systemUser = await db.Users.FirstOrDefaultAsync(x => x.IsSystemUser);
            if (systemUser == null)
            {
                systemUser = new User
                {
                    OrganizationId = systemOrganization.Id,
                    DepartmentId = systemDepartment.Id,
                    UserId = "system_uzon",
                    UserName = "系统用户",
                    // 系统用户无法登陆
                    Password = "system1234",
                    IsSystemUser = true,
                    IsHidden = true
                };

                db.Add(systemUser);
            }

            // 保存系统用户
            await db.SaveChangesAsync();

            var defaultOrganization = await db.Departments.FirstOrDefaultAsync(x => x.Type == DepartmentType.Organization && !x.IsSystem);
            // 添加默认组织
            if (defaultOrganization == null)
            {
                defaultOrganization = new Department
                {
                    Name = Department.DefaultOrganizationName,
                    Description = "默认组织",
                    FullPath = Department.DefaultDepartmentName,
                    IsSystem = false,
                    IsHidden = false,
                    Type = DepartmentType.Organization
                };
                db.Add(defaultOrganization);
            }

            // 添加默认部门
            var defaultDepartment = await db.Departments.FirstOrDefaultAsync(x => x.Type == DepartmentType.Department && !x.IsSystem);
            if (defaultDepartment == null)
            {
                defaultDepartment = new Department
                {
                    Name = Department.DefaultDepartmentName,
                    Description = "默认部门",
                    FullPath = $"{Department.DefaultOrganizationName}/{Department.DefaultDepartmentName}",
                    ParentId = 3,
                    IsSystem = false,
                    IsHidden = false,
                    Type = DepartmentType.Department
                };
                db.Add(defaultDepartment);
            }

            // 添加超管
            var adminUser = await db.Users.FirstOrDefaultAsync(x => x.IsSuperAdmin);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    OrganizationId = 3,
                    DepartmentId = 4,
                    UserId = "admin",
                    UserName = "admin",
                    // 密码是进行了 Sha256 二次加密
                    Password = "admin1234".Sha256(1),
                    IsSuperAdmin = true,
                    IsHidden = true,
                };

                // 从配置中读取超管的信息
                if (config?.User?.AdminUser != null)
                {
                    var adminUserConfig = config.User.AdminUser;
                    if (!string.IsNullOrEmpty(adminUserConfig.UserId)) adminUser.UserId = adminUserConfig.UserId;
                    if (!string.IsNullOrEmpty(adminUserConfig.Password)) adminUser.Password = adminUserConfig.Password.Sha256(1);
                    if (!string.IsNullOrEmpty(adminUserConfig.Avatar)) adminUser.Avatar = adminUserConfig.Avatar;
                }

                db.Add(adminUser);
            }
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

            // 添加初始数据
            // db.Users.AddRange();
        }

        /// <summary>
        /// 初始化存储库
        /// </summary>
        private async Task InitFileStorage(SqlContext db, AppConfig config)
        {
            bool existDefaultFileBucket = await db.FileBuckets.AnyAsync(x => x.IsDefault);
            if (existDefaultFileBucket) return;

            // 新建
            var defaultBucket = new FileBucket
            {
                BucketName = "default",
                Description = "默认存储桶",
                RootDir = config.FileStorage.DefaultRootDir,
                IsDefault = true
            };

            // 创建目录
            Directory.CreateDirectory(defaultBucket.RootDir);
            db.FileBuckets.Add(defaultBucket);
        }
    }
}
