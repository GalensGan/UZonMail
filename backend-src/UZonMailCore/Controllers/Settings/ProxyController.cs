using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using UZonMail.Utils.Web.Extensions;
using UZonMail.Utils.Web.ResponseModel;
using UZonMail.Core.Services.Settings;
using UZonMail.Core.Utils.ASPNETCore.PagingQuery;
using UZonMail.Core.Utils.DotNETCore.Exceptions;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Settings;
using UZonMail.DB.SQL.Emails;

namespace UZonMail.Core.Controllers.Settings
{
    /// <summary>
    /// 代理控制器
    /// </summary>
    public class ProxyController(SqlContext db, ProxyService proxyService, TokenService tokenService) : ControllerBaseV1
    {
        /// <summary>
        /// 验证代理名称是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        [HttpGet("valid-name")]
        public async Task<ResponseResult<bool>> ValidateProxyName(string name)
        {
            var isExist = await proxyService.ValidateProxyName(name);
            if (isExist)
            {
                return ResponseResult<bool>.Fail(isExist.Message);
            }
            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 创建代理
        /// </summary>
        /// <param name="userProxy"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<ResponseResult<UserProxy>> CreateProxy(UserProxy userProxy)
        {
            var isExist = await proxyService.ValidateProxyName(userProxy.Name);
            if (isExist)
            {
                return ResponseResult<UserProxy>.Fail(isExist.Message);
            }

            // 验证代理设置是否合法
            if (!ProxyInfo.CanParse(userProxy.Proxy))
            {
                return ResponseResult<UserProxy>.Fail("代理格式不正确");
            }

            var proxy = await proxyService.CreateUserProxy(userProxy);
            return proxy.ToSuccessResponse();
        }

        /// <summary>
        /// 更新代理
        /// </summary>
        /// <param name="userProxy"></param>
        /// <returns></returns>
        [HttpPut()]
        public async Task<ResponseResult<bool>> UpdateProxy(UserProxy userProxy)
        {
            var result = await proxyService.UpdateUserProxy(userProxy);
            return result.ToSuccessResponse();
        }

        /// <summary>
        /// 更新代理共享状态
        /// </summary>
        /// <param name="proxyId"></param>
        /// <param name="isShared"></param>
        /// <returns></returns>
        [HttpPut("{proxyId:long}/shared")]
        public async Task<ResponseResult<bool>> UpdateProxySharedStatus(long proxyId, bool isShared)
        {
            var result = await proxyService.UpdateUserProxySharedStatus(proxyId, isShared);
            return result.ToSuccessResponse();
        }

        /// <summary>
        /// 删除代理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:long}")]
        public async Task<ResponseResult<bool>> DeleteById(long id)
        {
            var userId = tokenService.GetUserDataId();
            db.UserProxies.Remove(new UserProxy { Id = id, UserId = userId });
            await db.SaveChangesAsync();
            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("filtered-count")]
        public async Task<ResponseResult<int>> GetFilteredCount(string filter)
        {
            var userId = tokenService.GetUserDataId();
            var dbSet = db.UserProxies.Where(x => x.UserId == userId);
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Name.Contains(filter) || x.Proxy.Contains(filter));
            }

            var count = await dbSet.CountAsync();
            return count.ToSuccessResponse();
        }

        /// <summary>
        /// 获取过滤后的数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpPost("filtered-data")]
        public async Task<ResponseResult<List<UserProxy>>> GetFilteredData(string filter, [FromBody] Pagination pagination)
        {
            var userId = tokenService.GetUserDataId();
            var dbSet = db.UserProxies.Where(x => x.UserId == userId);
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Name.Contains(filter) || x.Proxy.Contains(filter));
            }
            var results = await dbSet.Page(pagination).ToListAsync();
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 获取所有可用的代理
        /// </summary>
        /// <returns></returns>
        [HttpGet("usable")]
        public async Task<ResponseResult<List<UserProxy>>> GetAllUsableProxies()
        {
            var userId = tokenService.GetUserDataId();
            var dbSet = db.UserProxies.Where(x => x.UserId == userId || x.IsShared).Where(x => x.IsActive);
            var results = await dbSet.ToListAsync();
            return results.ToSuccessResponse();
        }
    }
}
