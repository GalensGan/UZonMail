using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Controllers.Emails.Models;
using UZonMailService.UzonMailDB.SQL;
using UZonMailService.UzonMailDB.SQL.EmailSending;
using UZonMailService.UzonMailDB.SQL.Templates;
using UZonMailService.Services.EmailSending;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.EmailSending.WaitList;
using UZonMailService.Utils.Database;
using UZonMailService.Utils.DotNETCore.Exceptions;

namespace UZonMailService.Controllers.Emails
{
    /// <summary>
    /// 发件相关接口
    /// </summary>
    public class EmailSendingController(SqlContext db
        , SendingGroupService sendingService
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
                return ResponseResult<SendingGroup>.Fail(vdResult.Message ?? "发件数据校验失败");
            }

            // 创建发件组
            sendingData.SendingType = SendingGroupType.Instant;
            var sendingGroup = await sendingService.CreateSendingGroup(sendingData);
            db.ChangeTracker.Clear();
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
            var sendingGroup = await db.SendingGroups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sendingGroupId);
            if (sendingGroup == null)
            {
                return false.ToErrorResponse("发件组不存在");
            }

            // 暂停发件
            await sendingService.RemoveSendingGroupTask(sendingGroup);

            // 更新状态
            await db.SendingGroups.UpdateAsync(x => x.Id == sendingGroupId, x => x.SetProperty(y => y.Status, SendingGroupStatus.Pause));
            await db.SendingItems.UpdateAsync(x => x.SendingGroupId == sendingGroupId && x.Status == SendingItemStatus.Pending, x => x.SetProperty(y => y.Status, SendingItemStatus.Created));

            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 重新开始发件
        /// </summary>
        /// <param name="sendingGroupId"></param>
        /// <returns></returns>
        [HttpPost("sending-groups/{sendingGroupId:long}/restart")]
        public async Task<ResponseResult<bool>> RestartSending(long sendingGroupId, [FromBody] SmtpSecretKeysModel smtpSecretKeys)
        {
            // 查找发件组
            var sendingGroup = await db.SendingGroups.AsNoTracking().Where(x => x.Id == sendingGroupId).FirstOrDefaultAsync();
            if (sendingGroup == null)
            {
                return false.ToErrorResponse("发件组不存在");
            }
            sendingGroup.SmtpPasswordSecretKeys = smtpSecretKeys.SmtpPasswordSecretKeys;

            // 重新开始发件
            await sendingService.SendNow(sendingGroup);

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
                await sendingService.RemoveSendingGroupTask(sendingGroup);
            }

            // 更新状态
            await db.SendingGroups.UpdateAsync(x => x.Id == sendingGroupId, x => x.SetProperty(y => y.Status, SendingGroupStatus.Cancel));
            await db.SendingItems.UpdateAsync(x => x.SendingGroupId == sendingGroupId && x.Status == SendingItemStatus.Pending, x => x.SetProperty(y => y.Status, SendingItemStatus.Cancel));

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
