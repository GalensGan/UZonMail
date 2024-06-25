using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Controllers.Emails.Models;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.EmailSending;
using UZonMailService.Models.SQL.Templates;
using UZonMailService.Services.EmailSending;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.EmailSending.WaitList;
using UZonMailService.Utils.DotNETCore.Exceptions;

namespace UZonMailService.Controllers.Emails
{
    /// <summary>
    /// 发件相关接口
    /// </summary>
    public class EmailSendingController(SqlContext db
        , SendingGroupService sendingService
        , SystemSendingWaitListService waitList
        , SystemTasksService tasksService
        ) : ControllerBaseV1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("now")]
        public async Task<ResponseResult<SendingGroup>> SendNow([FromBody] SendingGroup sendingData)
        {
            // 校验数据
            var vdResult = sendingData.Validate();
            if (!vdResult)
            {
                return ResponseResult<SendingGroup>.Fail(vdResult.Message);
            }

            // 创建发件组
            sendingData.SendingType = SendingGroupType.Instant;
            var sendingGroup = await sendingService.CreateSendingGroup(sendingData);
            await sendingService.SendNow(sendingGroup);

            return new SendingGroup()
            {
                Id = sendingGroup.Id,
                TotalCount = sendingGroup.TotalCount
            }.ToSuccessResponse();
        }

        /// <summary>
        /// 计划发送
        /// </summary>
        /// <param name="sendingData"></param>
        /// <returns></returns>
        [HttpPost("schedule")]
        public async Task<ResponseResult<SendingGroup>> SendSchedule([FromBody] SendingGroup sendingData)
        {
            // 校验数据
            var vdResult = sendingData.Validate();
            if (!vdResult)
            {
                return ResponseResult<SendingGroup>.Fail(vdResult.Message);
            }

            sendingData.SendingType = SendingGroupType.Scheduled;
            // 创建发件组
            var sendingGroup = await sendingService.CreateSendingGroup(sendingData);
            await sendingService.SendSchedule(sendingGroup);

            return new SendingGroup()
            {
                Id = sendingGroup.Id,
                TotalCount = sendingGroup.TotalCount
            }.ToSuccessResponse();
        }

        [HttpPost("sending-groups/{sendingGroupId:long}/pause")]
        public async Task<ResponseResult<bool>> PauseSending(long sendingGroupId)
        {
            // 查找发件组
            var sendingGroup = await db.SendingGroups.FirstOrDefaultAsync(x => x.Id == sendingGroupId);
            if (sendingGroup == null)
            {
                return false.ToErrorResponse("发件组不存在");
            }

            // 暂停发件
            waitList.SwitchSendTaskStatus(sendingGroup, true);

            // 更新状态
            sendingGroup.Status = SendingGroupStatus.Pause;
            await db.SaveChangesAsync();

            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 重新开始发件
        /// </summary>
        /// <param name="sendingGroupId"></param>
        /// <returns></returns>
        [HttpPost("sending-groups/{sendingGroupId:long}/restart")]
        public async Task<ResponseResult<bool>> RestartSending(long sendingGroupId)
        {
            // 查找发件组
            var sendingGroup = await db.SendingGroups.FirstOrDefaultAsync(x => x.Id == sendingGroupId);
            if (sendingGroup == null)
            {
                return false.ToErrorResponse("发件组不存在");
            }

            // 暂停发件
            waitList.SwitchSendTaskStatus(sendingGroup, false);
            sendingGroup.Status = SendingGroupStatus.Sending;
            await db.SaveChangesAsync();

            // 开始发件
            tasksService.StartSending();

            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 取消发件
        /// </summary>
        /// <param name="sendingGroupId"></param>
        /// <returns></returns>
        [HttpPost("sending-groups/{sendingGroupId:long}/cancel")]
        public async Task<ResponseResult<bool>> CancelSending(long sendingGroupId)
        {
            // 查找发件组
            var sendingGroup = await db.SendingGroups.FirstOrDefaultAsync(x => x.Id == sendingGroupId);
            if (sendingGroup == null)
            {
                return false.ToErrorResponse("发件组不存在");
            }

            // 若处于发送中，则取消
            if (sendingGroup.Status == SendingGroupStatus.Sending)
            {
                // 取消发件
                await waitList.CancelSending(sendingGroup);
            }

            // 更新状态
            sendingGroup.Status = SendingGroupStatus.Cancel;
            await db.SaveChangesAsync();

            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 重新发送某一封邮件
        /// </summary>
        /// <param name="sendingItemId"></param>
        /// <returns></returns>
        [HttpPost("sending-items/{sendingItemId:long}/resend")]
        public async Task<ResponseResult<bool>> ResendSendingItem(long sendingItemId, [FromBody] SmtpSecretKeysModel smtpSecretKeys)
        {
            var sendingItem = await db.SendingItems.Where(x => x.Id == sendingItemId)
                .Include(x => x.SendingGroup)
                .FirstOrDefaultAsync();
            if (sendingItem == null)
            {
                return false.ToErrorResponse("发件项不存在");
            }

            // 查找发件项
            var sendingGroup = sendingItem.SendingGroup;
            if (sendingGroup == null)
            {
                return false.ToErrorResponse("发件组不存在");
            }            
            if (sendingGroup.SuccessCount == sendingGroup.TotalCount)
            {
                return false.ToErrorResponse("发件组已全部成功，不支持重发");
            }

            // 开始发件
            sendingGroup.SmtpPasswordSecretKeys = smtpSecretKeys.SmtpPasswordSecretKeys;
            await sendingService.SendNow(sendingGroup, [sendingItem.Id]);

            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 重新发送整个发件组
        /// </summary>
        /// <param name="sendingGroupId"></param>
        /// <returns></returns>
        [HttpPost("sending-groups/{sendingGroupId:long}/resend")]
        public async Task<ResponseResult<bool>> ResendSendingGroup(long sendingGroupId, [FromBody] SmtpSecretKeysModel smtpSecretKeys)
        {
            // 查找发件项
            var sendingGroup = await db.SendingGroups.FirstOrDefaultAsync(x => x.Id == sendingGroupId);
            if (sendingGroup == null)
            {
                return false.ToErrorResponse("发件组不存在");
            }
            if (sendingGroup.Status != SendingGroupStatus.Finish)
            {
                return false.ToErrorResponse("发件组未结束，不支持重发");
            }
            if (sendingGroup.SuccessCount == sendingGroup.TotalCount)
            {
                return false.ToErrorResponse("发件组已全部成功，不支持重发");
            }

            // 重新发送
            sendingGroup.SmtpPasswordSecretKeys = smtpSecretKeys.SmtpPasswordSecretKeys;
            await sendingService.SendNow(sendingGroup);
            return true.ToSuccessResponse();
        }
    }
}
