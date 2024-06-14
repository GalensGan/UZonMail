using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;
using Uamazing.Utils.Web.Service;
using UZonMailService.Config;
using Uamazing.Utils.Web.Token;
using Uamazing.Utils.Json;

namespace UZonMailService.Services.Settings
{
    /// <summary>
    /// Token 相关的服务
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    /// <param name="appConfig"></param>
    public class TokenService(IHttpContextAccessor httpContextAccessor, IOptions<AppConfig> appConfig) : IScopedService
    {
        private HttpRequest Request => httpContextAccessor.HttpContext.Request;
        /// <summary>
        /// 获取 token 值
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public string GetToken()
        {
            string tokenHeader = Request.Headers[HeaderNames.Authorization].ToString();
            if (string.IsNullOrEmpty(tokenHeader))
                throw new ArgumentNullException("缺少token!");

            string pattern = "^Bearer (.*?)$";
            if (!Regex.IsMatch(tokenHeader, pattern))
                throw new Exception("token格式不对!格式为:Bearer {token}");

            string? token = Regex.Match(tokenHeader, pattern)?.Groups[1]?.ToString();
            if (string.IsNullOrEmpty(token))
                throw new Exception("token不能为空!");

            return token;
        }

        /// <summary>
        /// 获取 token 中的 userId
        /// </summary>
        /// <returns></returns>
        public int GetIntUserId()
        {
            var token = GetToken();
            var userId = appConfig.Value.TokenParams.GetTokenPayloads(token).SelectTokenOrDefault("userId", "");
            if(int.TryParse(userId, out int intUserId)) return intUserId;
            return 0;
        }
    }
}
