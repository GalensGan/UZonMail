using Microsoft.AspNetCore.Mvc;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;

namespace UZonMailService.Controllers.Sponsor
{
    /// <summary>
    /// 赞助相关接口
    /// </summary>
    public class SponsorController(IHttpClientFactory httpClientFactory) : ControllerBaseV1
    {
        /// <summary>
        /// 获取赞助页面的 html
        /// </summary>
        /// <returns></returns>
        [HttpGet("content")]
        public async Task<ResponseResult<string>> GetSponsorPageHtml()
        {
            // 从 https://gitee.com/galensgan/SendMultipleEmails/raw/master/docs/sponsor.md 读取内容
            string url = "https://gitee.com/galensgan/SendMultipleEmails/raw/master/docs/sponsor.md";
            var httpClient = httpClientFactory.CreateClient();
            var content = await httpClient.GetStringAsync(url);
            return content.ToSuccessResponse();
        }
    }
}
