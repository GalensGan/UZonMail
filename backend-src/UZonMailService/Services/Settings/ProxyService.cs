using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Results;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Settings;
using UZonMailService.Utils.Database;
using UZonMailService.Utils.DotNETCore.Exceptions;

namespace UZonMailService.Services.Settings
{
    /// <summary>
    /// 代理服务
    /// </summary>
    public class ProxyService(SqlContext db, TokenService tokenService) : IScopedService
    {
        /// <summary>
        /// 验证代理名称是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public async Task<StringResult> ValidateProxyName(string name)
        {
            if (string.IsNullOrEmpty(name)) return StringResult.Fail("代理名称不能为空");

            int userId = tokenService.GetIntUserId();
            bool isExist = await db.UserProxies.AnyAsync(x => x.UserId == userId && x.Name == name);
            return new StringResult(isExist, "代理名称已存在");
        }

        /// <summary>
        /// 创建用户代理
        /// </summary>
        /// <param name="userProxy"></param>
        /// <returns></returns>
        public async Task<UserProxy> CreateUserProxy(UserProxy userProxy)
        {
            var userId = tokenService.GetIntUserId();
            userProxy.UserId = userId;
            userProxy.IsActive = true;
            db.UserProxies.Add(userProxy);
            await db.SaveChangesAsync();
            return userProxy;
        }

        /// <summary>
        /// 更新代理
        /// </summary>
        /// <param name="userProxy"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserProxy(UserProxy userProxy)
        {
            var userId = tokenService.GetIntUserId();
            await db.UserProxies.UpdateAsync(x => x.UserId == userId && x.Id == userProxy.Id,
                x => x.SetProperty(y => y.Name, userProxy.Name)
                    .SetProperty(y => y.Description, userProxy.Description)
                    .SetProperty(y => y.Proxy, userProxy.Proxy)
                    .SetProperty(y => y.IsActive, userProxy.IsActive)
                    .SetProperty(y => y.MatchRegex, userProxy.MatchRegex)
                    .SetProperty(y => y.Priority, userProxy.Priority)
                );
            return true;
        }

        /// <summary>
        /// 更新代理共享状态
        /// </summary>
        /// <param name="proxyId"></param>
        /// <param name="statusValue"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserProxySharedStatus(int proxyId,bool statusValue)
        {
            var userId = tokenService.GetIntUserId();
            await db.UserProxies.UpdateAsync(x => x.UserId == userId && x.Id == proxyId,
                x => x.SetProperty(y => y.IsShared, statusValue)
                );
            return true;
        }
    }
}
