using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;
using UZonMail.Utils.Web.Service;
using UZonMail.Core.Config;
using UZonMail.Utils.Web.Token;
using UZonMail.Utils.Json;
using Newtonsoft.Json.Linq;

namespace UZonMail.Core.Services.Settings
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
        public long GetUserDataId()
        {
            var token = GetToken();
            var userId = appConfig.Value.TokenParams.GetTokenPayloads(token).SelectTokenOrDefault("userId", "");
            if(long.TryParse(userId, out long intUserId)) return intUserId;
            return 0L;
        }

        /// <summary>
        /// 获取 token 中的数据
        /// </summary>
        /// <returns></returns>
        public TokenPayloads GetTokenPayloads()
        {
            var token = GetToken();
            var paylods = appConfig.Value.TokenParams.GetTokenPayloads(token);
            return new TokenPayloads(paylods);
        }
    }
}
