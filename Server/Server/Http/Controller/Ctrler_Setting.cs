using EmbedIO;
using EmbedIO.Routing;
using LiteDB;
using Newtonsoft.Json.Linq;
using Server.Database.Definitions;
using Server.Database.Extensions;
using Server.Database.Models;
using Server.SDK.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    public class Ctrler_Setting : BaseController
    {
        [Route(HttpVerbs.Get, "/setting")]
        public void GetUserSettings()
        {
            Setting setting = LiteDb.SingleOrDefault<Setting>(s => s.userId == Token.UserId);
            ResponseSuccess(setting);
        }

        /// <summary>
        /// 设置
        /// </summary>
        [Route(HttpVerbs.Put, "/setting/send-interval")]
        public async Task SendIntervalChanged()
        {
            var body = JObject.Parse(await HttpContext.GetRequestBodyAsStringAsync());

            double max = body.SelectToken("sendInterval.max").ValueOrDefault(8d);
            double min = body.SelectToken("sendInterval.min").ValueOrDefault(3d);

            // 判断是否存在设置项
            LiteDb.Upsert2(s => s.userId == Token.UserId, new Setting()
            {
                userId = Token.UserId,
                sendInterval_max = max,
                sendInterval_min = min,
            }, new UpdateOptions() {"sendInterval_max", "sendInterval_min" });

            // 返回成功
            ResponseSuccess("success");
        }

        /// <summary>
        /// 自动重发
        /// </summary>
        /// <returns></returns>
        [Route(HttpVerbs.Put, "/setting/auto-resend")]
        public async Task IsAutoResendChanged()
        {
            var body = JObject.Parse(await HttpContext.GetRequestBodyAsStringAsync());

            bool isAutoResend = body.SelectToken("isAutoResend").ValueOrDefault(true);

            // 判断是否存在设置项
            LiteDb.Upsert2(s => s.userId == Token.UserId, new Setting()
            {
                userId = Token.UserId,
                isAutoResend = isAutoResend,
            }, new UpdateOptions() { "isAutoResend" });

            // 返回成功
            ResponseSuccess("success");
        }

        /// <summary>
        /// 自动重发
        /// </summary>
        /// <returns></returns>
        [Route(HttpVerbs.Put, "/setting/send-with-image-html")]
        public async Task SendWithImageAndHtmlChanged()
        {
            var body = JObject.Parse(await HttpContext.GetRequestBodyAsStringAsync());

            bool sendWithImageAndHtml = body.SelectToken("sendWithImageAndHtml").ValueOrDefault(false);

            // 判断是否存在设置项
            LiteDb.Upsert2(s => s.userId == Token.UserId, new Setting()
            {
                userId = Token.UserId,
                sendWithImageAndHtml = sendWithImageAndHtml,
            }, new UpdateOptions() { "sendWithImageAndHtml" });

            // 返回成功
            ResponseSuccess("success");
        }
    }
}
