using EmbedIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    /// <summary>
    /// 具有异步的 controller 基类
    /// </summary>
    public class BaseControllerAsync : BaseController
    {
        // 异步发送数据
        private async Task SendJObjectAsync(object obj)
        {
            Response.ContentType = MimeType.Json;
            using (var writer = HttpContext.OpenResponseText(Encoding.UTF8, true))
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(obj));
            }
        }

        /// <summary>
        /// 发送成功的回应
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task ResponseSuccessAsync(object data)
        {
            var jobj = new HttpResponse()
            {
                data = data,
            };

            await SendJObjectAsync(jobj);
        }

        /// <summary>
        /// 发送失败回应
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        protected async Task ResponseErrorAsync(string errorMsg)
        {
            var jobj = new HttpResponse()
            {
                code = 204,
                message = errorMsg,
            };
           await SendJObjectAsync(jobj);
        }
    }
}
