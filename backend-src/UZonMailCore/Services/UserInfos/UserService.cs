using Microsoft.EntityFrameworkCore;
using UZonMail.Utils.Web.Service;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.MultiTenant;
using UZonMail.Utils.Extensions;
using UZonMail.Utils.Web.Extensions;
using UZonMail.Core.Utils.DotNETCore.Exceptions;
using UZonMail.Utils.Web.Token;
using UZonMail.Core.Config;
using Microsoft.Extensions.Options;
using UZonMail.Utils.Web.ResponseModel;
using UZonMail.Core.Utils.ASPNETCore.PagingQuery;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using UZonMail.Core.Services.Settings;
using System.Net.NetworkInformation;
using UZonMail.Core.Controllers.Users.Model;
using UZonMail.Core.Cache;
using UZonMail.Core.Services.Permission;

namespace UZonMail.Core.Services.UserInfos
{
    /// <summary>
    /// 只在请求生命周期内有效的服务
    /// </summary>
    public class UserService(SqlContext db, IOptions<AppConfig> appConfig, PermissionService permission) : IScopedService
    {
        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> ExistUser(string userId)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.UserId.ToLower() == userId.ToLower()) != null;
        }

        /// <summary>
        /// 创建一个新的用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password">原密码,不需要加密</param>
        /// <returns></returns>
        public async Task<User> CreateUser(string userId, string password)
        {
            var user = new User()
            {
                UserId = userId,
                Password = password.Sha256(1)
            };
            db.Add(user);
            await db.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// 通过用户 ID 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User> GetUserInfo(int userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId) ?? throw new KnownException(userId + "用户不存在");
            // 将密码置空
            user.Password = string.Empty;
            return user;
        }

        /// <summary>
        /// 通过 userId 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public async Task<User> GetUserInfo(string userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId) ?? throw new KnownException(userId + "用户不存在");
            // 将密码置空
            user.Password = string.Empty;
            return user;
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password">密码为原值</param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public async Task<UserSignInResult> UserSignIn(string userId, string password)
        {
            password = password.Sha256();
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId && x.Password == password)
                ?? throw new KnownException("用户名或密码错误");
            var userInfo = new User()
            {
                Id = user.Id,
                Avatar = user.Avatar,
                UserId = user.UserId,
            };

            // 生成 token
            string token = GenerateToken(user);

            // 查找用户的权限
            List<string> access = await permission.GetUserPermissionCodes(user.Id);

            return new UserSignInResult()
            {
                Token = token,
                Access = access.Distinct().ToList(),
                UserInfo = userInfo
            };
        }
        /// <summary>
        /// 生成 token
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private string GenerateToken(User userInfo)
        {
            // 保证每个机器不一样
            string token = JWTToken.CreateToken(appConfig.Value.TokenParams, new TokenPayloads(userInfo));
            return token;
        }

        /// <summary>
        /// 获取用户数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetFilteredUsersCount(string filter)
        {
            return await FilterUser(filter).CountAsync();
        }
        private IQueryable<User> FilterUser(string filter)
        {
            return db.Users.Where(x => !x.IsDeleted && !x.IsHidden && !x.IsSuperAdmin)
                .Where(x => string.IsNullOrEmpty(filter) || x.UserId.Contains(filter));
        }

        /// <summary>
        /// 获取分页的用户
        /// 返回除了密码之外的所有信息
        /// 调用接口对数据进行再处理
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetFilteredUsersData(string filter, Pagination pagination)
        {
            return await FilterUser(filter).Page(pagination).ToListAsync();
        }

        /// <summary>
        /// 获取用户默认密码
        /// </summary>
        /// <returns></returns>
        public string GetUserDefaultPassword()
        {
            return appConfig.Value.User.DefaultPassword;
        }

        /// <summary>
        /// 重置用户密码
        /// 由于无法知道用户的原始密码，因此重置后，发件箱的 smtp 密码将无法被解密
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public async Task<bool> ResetUserPassword(string userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId) ?? throw new KnownException("用户不存在");
            user.Password = appConfig.Value.User.DefaultPassword.Sha256(1);
            await db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> ChangeUserPassword(long userId, ChangePasswordModel passwordModel)
        {
            var oldPassword = passwordModel.OldPassword;
            var newPassword = passwordModel.NewPassword;

            if (userId <= 0 || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                throw new KnownException("新旧密码不能为空");
            }

            // 查找用户
            string encryptOldPassword = oldPassword.Sha256();
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId && x.Password == encryptOldPassword) ?? throw new KnownException("原密码错误");
            string encryptNewPassword = newPassword.Sha256();

            // 在事务中修改密码
            await db.RunTransaction(async ctx =>
            {
                user.Password = encryptNewPassword;

                // 对 smtp 的密码先使用原密码解密，然后再用新密码加密
                var outboxes = await db.Outboxes.Where(x => x.UserId == userId).ToListAsync();
                foreach (var outbox in outboxes)
                {
                    // 原密钥
                    var smtpPassword = DecryptSmtpPassword(outbox.Password, passwordModel.OldSmtpPasswordSecretKeys);

                    // 计算新的密码
                    outbox.Password = EncryptSmtpPassword(smtpPassword, passwordModel.NewSmtpPasswordSecretKeys);
                }

                await db.SaveChangesAsync();
                return true;
            });

            return true;
        }

        /// <summary>
        /// 加密 smtp 密码
        /// </summary>
        /// <param name="password"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string EncryptSmtpPassword(string password, SmtpPasswordSecretKeys secretKeys)
        {
            return password.AES(secretKeys.Key, secretKeys.Iv);
        }

        /// <summary>
        /// 解密 smtp 密码
        /// </summary>
        /// <param name="password"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string DecryptSmtpPassword(string password, SmtpPasswordSecretKeys secretKeys)
        {
            return password.DeAES(secretKeys.Key, secretKeys.Iv);
        }
    }
}
