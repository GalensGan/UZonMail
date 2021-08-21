using EmbedIO;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Database;
using Server.Http.Extensions;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    public abstract class BaseController : WebApiController
    {
        protected IContainer IoC { get; private set; }

        protected LiteDBManager LiteDb { get; private set; }

        /// <summary>
        /// 获取控制数据库操作类
        /// </summary>
        protected override void OnBeforeHandler()
        {
            IoC = HttpContext.GetIoCScope();
            LiteDb = IoC.Get<LiteDBManager>();

            base.OnBeforeHandler();
        }

        /// <summary>
        /// 发送成功的回应
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task ResponseSuccess(object data)
        {
            var jobj = new JObject(new JProperty("code", 20000), new JProperty("data", JObject.FromObject(data)));
            await SendJObject(jobj);
        }

        /// <summary>
        /// 发送失败回应
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        protected async Task ResponseError(string errorMsg)
        {
            var jobj = new JObject(new JProperty("code", 20001), new JProperty("message", errorMsg));
            await SendJObject(jobj);
        }

        private async Task SendJObject(JObject jObject)
        {
            await HttpContext.SendStringAsync(JsonConvert.SerializeObject(jObject), "application/json", Encoding.UTF8);
        }
    }
}
