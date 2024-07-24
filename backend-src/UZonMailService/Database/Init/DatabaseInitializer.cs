using Microsoft.Extensions.Hosting;
using UZonMailService.Config;
using UZonMailService.Config.SubConfigs;
using Uamazing.Utils.Extensions;
using UZonMailService.UzonMailDB.SQL.Files;
using Microsoft.EntityFrameworkCore;
using UZonMailService.UzonMailDB.SQL.Settings;
using UZonMailService.UzonMailDB.SQL.EmailSending;
using UZonMailService.Utils.Database;
using UZonMailService.UzonMailDB.SQL.MultiTenant;

namespace UZonMailService.Database.Init
{
    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="hostEnvironment"></param>
    /// <param name="sqlContext"></param>
    /// <param name="config"></param>
    public class DatabaseInitializer(IWebHostEnvironment hostEnvironment, SqlContext sqlContext, AppConfig config)
    {
        private readonly SqlContext _db = sqlContext;
        private IWebHostEnvironment _hostEnvironment = hostEnvironment;
        private AppConfig _appConfig = config;

        /// <summary>
        /// 开始执行初始化
        /// </summary>
        public async Task Init()
        {
            await InitUser();
            await InitPermission();
            await InitFileStorage();
            await ResetSendingItemsStatus();
            await ResetSendingGroup();

            _db.SaveChanges();
        }

        private async Task InitUser()
        {
            // 判断是否有默认组织和部门
            var systemDepartments = await _db.Departments.Where(x => x.IsSystem).ToListAsync();
            string systemOrgName = Department.GetSystemOrganizationName();
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
                    IsHidden=true,
                };
                _db.Add(systemOrganization);
            }

            // 判断是否有系统级部门
            string systemDpName = Department.GetSystemDepartmentName();
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
                };
                _db.Add(systemDepartment);
            }

            // 设置系统用户，用于挂载系统相关的数据
            var systemUser = await _db.Users.FirstOrDefaultAsync(x => x.IsSystemUser);
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

                _db.Add(systemUser);
            }

            // 设置超管,防止其它账号权限被撤销
            var adminUser = await _db.Users.FirstOrDefaultAsync(x => x.IsSuperAdmin);
            if (adminUser == null)
            {
                adminUser = new MultiTenant.User
                {
                    OrganizationId = systemOrganization.Id,
                    DepartmentId = systemDepartment.Id,
                    UserId = "admin",
                    UserName = "admin",
                    // 密码是进行了 Sha256 二次加密
                    Password = "admin1234".Sha256(1),
                    IsSuperAdmin = true,
                    IsHidden = true,
                };

                // 从配置中读取超管的信息
                if (_appConfig?.User?.AdminUser != null)
                {
                    var adminUserConfig = _appConfig.User.AdminUser;
                    if (!string.IsNullOrEmpty(adminUserConfig.UserId)) adminUser.UserId = adminUserConfig.UserId;
                    if (!string.IsNullOrEmpty(adminUserConfig.Password)) adminUser.Password = adminUserConfig.Password.Sha256(1);
                    if (!string.IsNullOrEmpty(adminUserConfig.Avatar)) adminUser.Avatar = adminUserConfig.Avatar;
                }

                _db.Add(adminUser);
            }
        }

        /// <summary>
        /// 初始化用户角色
        /// 1. 添加默认功能码,包含管理员
        /// 2. 生成一个管理员角色和一个普通用户角色
        /// 3. 为角色赋予默认功能码
        /// </summary>
        private async Task InitPermission()
        {
            // 检查是否已经有数据
            if (_db.Users.Any())
            {
                return;   // 如果已经有数据，直接返回
            }

            // 添加初始数据
            _db.Users.AddRange();
        }

        /// <summary>
        /// 初始化存储库
        /// </summary>
        private async Task InitFileStorage()
        {
            bool existDefaultFileBucket = await _db.FileBuckets.AnyAsync(x => x.IsDefault);
            if (existDefaultFileBucket) return;

            // 新建
            var defaultBucket = new FileBucket
            {
                BucketName = "default",
                Description = "默认存储桶",
                RootDir = _appConfig.FileStorage.DefaultRootDir,
                IsDefault = true
            };

            // 创建目录
            Directory.CreateDirectory(defaultBucket.RootDir);
            _db.FileBuckets.Add(defaultBucket);
        }

        private async Task ResetSendingItemsStatus()
        {
            // 对所有的 Pending 状态的发件项重置为 Created
            await _db.SendingItems.UpdateAsync(x => x.Status == SendingItemStatus.Pending, x => x.SetProperty(y => y.Status, SendingItemStatus.Created));          
        }

        private async Task ResetSendingGroup()
        {
            // 将所有的 Sending 或者 Create 状态的发件组重置为 Finish
            await _db.SendingGroups.UpdateAsync(x => x.Status == SendingGroupStatus.Sending
                || (x.Status == SendingGroupStatus.Created && x.Status != SendingGroupStatus.Scheduled)
            , obj => obj.SetProperty(x => x.Status, SendingGroupStatus.Finish));
        }
    }
}
