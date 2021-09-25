using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json.Linq;
using Server.Config;
using Server.Database;
using Server.Database.Models;
using Server.Http.Headers;
using Server.Http.Response;
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
        /// 有 async ，必须要有 task，否则资源会提前释放
        /// </summary>
        /// <returns></returns>
        [Route(HttpVerbs.Post, "/user/login")]
        public async Task UserLogin()
        {
            // 读取jsonData
            var body = JObject.Parse(await HttpContext.GetRequestBodyAsStringAsync());

            string userId = body.SelectToken(Fields.userName).ValueOrDefault(string.Empty);
            string password = body.SelectToken(Fields.password).ValueOrDefault(string.Empty); // 由于是客户端，不加密

            // 判断数据正确性
            if (string.IsNullOrEmpty(userId))
            {
                ResponseError("用户名为空");
                
            }

            if (string.IsNullOrEmpty(password))
            {
                ResponseError("密码为空");
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
                    ResponseError("密码错误");
                    return;
                }
            }

            UserConfig uConfig = IoC.Get<UserConfig>();
            JwtToken jwtToken = new JwtToken(uConfig.TokenSecret, userId, JwtToken.DefaultExp());

            ResponseSuccess(new JObject(new JProperty(Fields.token, jwtToken.Token)));
        }

        /// <summary>
        /// 获取用户信息
        /// 此处根据 element-admin 框架返回，避免修改其框架内容
        /// </summary>
        /// <returns></returns>
        [Route(HttpVerbs.Get, "/user/info")]
        public void UserInfo([QueryField] string token)
        {
            // 用 token 获取用户信息
            UserConfig uConfig = IoC.Get<UserConfig>();
            JwtToken jwtToken = new JwtToken(uConfig.TokenSecret, token);
            if (jwtToken.TokenValidState!=TokenValidState.Valid)
            {
                ResponseError("token无效");
                return;
            }
           

            // 返回用户信息
            var user = LiteDb.Query<User>().Where(u => u.userId == jwtToken.UserId).FirstOrDefault();
            if (user == null)
            {
                ResponseError("未找到用户！");
                return;
            }

            if (string.IsNullOrEmpty(user.avatar)) user.avatar = uConfig.DefaultAvatar;

            ResponseSuccess(user);
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route(HttpVerbs.Put, "/user/logout")]
        public void UserLogout()
        {
            ResponseSuccess("success");
        }
    }
}
