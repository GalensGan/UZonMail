using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json.Linq;
using Server.Config;
using Server.Database;
using Server.Database.Definitions;
using Server.Database.Extensions;
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
    public class Ctrler_User : BaseControllerAsync
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
                await ResponseErrorAsync("用户名为空");
            }

            if (string.IsNullOrEmpty(password))
            {
                await ResponseErrorAsync("密码为空");
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

                // 新建用户后，同时给用户建立默认配置
                LiteDb.Insert(Setting.DefaultSetting(userId));
            }
            else
            {
                // 判断密码正确性
                if (user.password != password)
                {
                    await ResponseErrorAsync("密码错误");
                    return;
                }
            }

            UserConfig uConfig = IoC.Get<UserConfig>();
            JwtToken jwtToken = new JwtToken(uConfig.TokenSecret, userId, JwtToken.DefaultExp());

            await ResponseSuccessAsync(new JObject(new JProperty(Fields.token, jwtToken.Token)));
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
            JwtToken jwtToken = new JwtToken(uConfig.TokenSecret, token);
            if (jwtToken.TokenValidState != TokenValidState.Valid)
            {
                await ResponseErrorAsync("token无效");
                return;
            }


            // 返回用户信息
            var user = LiteDb.Query<User>().Where(u => u.userId == jwtToken.UserId).FirstOrDefault();
            if (user == null)
            {
                await ResponseErrorAsync("未找到用户！");
                return;
            }

            if (string.IsNullOrEmpty(user.avatar)) user.avatar = uConfig.DefaultAvatar;

            await ResponseSuccessAsync(user);
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route(HttpVerbs.Put, "/user/logout")]
        public async Task UserLogout()
        {
            await ResponseSuccessAsync("success");
        }


        /// <summary>
        /// 更新用户的头像
        /// </summary>
        [Route(HttpVerbs.Put, "/user/avatar")]
        public async Task UpdateUserAvatar()
        {
            // 获取 body 传输的 url
            var avatarUrl = Body.SelectToken(Fields.avatar).ValueOrDefault(string.Empty);
            var userId = Body.SelectToken(Fields.userId).ValueOrDefault(string.Empty);

            if (string.IsNullOrEmpty(avatarUrl))
            {
                await ResponseErrorAsync("请在 data 中传入 avatarUrl");
                return;
            }

            if (string.IsNullOrEmpty(userId))
            {
                await ResponseErrorAsync("请在 data 中传入 userName");
                return;
            }

            var user = LiteDb.Fetch<User>(u => u.userId == userId).FirstOrDefault();
            if (user == null)
            {
                await ResponseErrorAsync("用户不存在");
                return;
            }

            // 更新头像
            LiteDb.Upsert2(s => s.userId == Token.UserId, new User()
            {
                avatar = avatarUrl,
            }, new UpdateOptions() { Fields.avatar });

            await ResponseSuccessAsync("success");
        }
    }
}
