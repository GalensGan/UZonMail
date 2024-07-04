using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Controllers.SystemInfo.Model;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.EmailSending.WaitList;

namespace UZonMailService.Controllers.SystemInfo
{
    public class SystemInfoController(UserSendingGroupsManager userSendingGroupsManager
        , UserOutboxesPoolManager userOutboxesPoolManager
        , SendingThreadManager sendingThreadManager) : ControllerBaseV1
    {
        /// <summary>
        /// 仅管理员可访问
        /// </summary>
        /// <returns></returns>
        [HttpGet("resource-usage")]
        //[Authorize("RequireAdmin")]
        [AllowAnonymous]
        public async Task<ResponseResult<SystemUsageInfo>> GetSystemResourceUsage()
        {
            var usageInfo = new SystemUsageInfo();
            await usageInfo.GatherInfomations(userSendingGroupsManager, userOutboxesPoolManager, sendingThreadManager);
            return usageInfo.ToSuccessResponse();
        }
    }
}
