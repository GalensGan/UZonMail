using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json.Linq;
using Server.Config;
using Server.Database;
using Server.Database.Definitions;
using Server.SDK.Extension;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    public class Ctrler_User : BaseController
    {
        /// <summary>
        /// 用户登陆
        /// 获取参数可以使用 [FormData] NameValueCollection data 传参
        /// </summary>
        /// <returns></returns>
        [Route(HttpVerbs.Post, "/user/login")]
        public async Task UserLogin()
        {
            // 读取jsonData
            var Body = JObject.Parse(await HttpContext.GetRequestBodyAsStringAsync());

            string userId = Body.SelectToken("username").ValueOrDefault(string.Empty);
            string password = Body.SelectToken("password").ValueOrDefault(""); // 由于是客户端，不加密

            // 判断数据正确性
            if (string.IsNullOrEmpty(userId))
            {
                await ResponseError("用户名为空");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                await ResponseError("密码为空");
                return;
            }

            // 获取数据库
            var user = LiteDb.Query<User>().Where(u => u.userId == userId).FirstOrDefault();
            if (user == null)
            {
                // 新建用户
                LiteDb.Insert(new User()
                {
                    userId = userId,
                    password = password,
                    createDate = DateTime.Now
                });
            }
            else
            {
                // 判断密码正确性
                if (user.password != password)
                {
                    await ResponseError("密码错误");
                    return;
                }
            }

            UserConfig uConfig = IoC.Get<UserConfig>();
            string token = Helpers.JWTHelper.CreateJwtToken(new Dictionary<string, object>()
            {
               {
                "userId",
                userId
               }
            }, uConfig.TokenSecret);

            await ResponseSuccess(new JObject(new JProperty("token",token)));
        }

        /// <summary>
        /// 获取用户信息
        /// 此处根据 element-admin 框架返回，避免修改其框架内容
        /// </summary>
        /// <returns></returns>
        [Route(HttpVerbs.Get, "/user/info")]
        public async Task UserInfo([QueryField] string token)
        {
            // 用 token 获取用户信息
            UserConfig uConfig = IoC.Get<UserConfig>();
            if (!Helpers.JWTHelper.ValidateJwtToken(token,uConfig.TokenSecret,out string result))
            {
                await ResponseError(result);
                return;
            }
            var jobj = JObject.Parse(result);

            string userId = jobj.SelectToken("userId").ValueOrDefault(string.Empty);

            // 返回用户信息
            var user = LiteDb.Query<User>().Where(u => u.userId == userId).FirstOrDefault();
            if (user == null)
            {
                await ResponseError("未找到用户！");
                return;
            }

            if (string.IsNullOrEmpty(user.avatar)) user.avatar = uConfig.DefaultAvatar;

            await ResponseSuccess(user);
        }
    }
}
