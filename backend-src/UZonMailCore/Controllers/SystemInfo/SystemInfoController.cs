using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UZonMail.Utils.Web.Extensions;
using UZonMail.Utils.Web.ResponseModel;
using UZonMail.Core.Controllers.SystemInfo.Model;
using UZonMail.Core.Services.EmailSending.OutboxPool;
using UZonMail.Core.Services.EmailSending.Sender;
using UZonMail.Core.Services.EmailSending.WaitList;

namespace UZonMail.Core.Controllers.SystemInfo
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
