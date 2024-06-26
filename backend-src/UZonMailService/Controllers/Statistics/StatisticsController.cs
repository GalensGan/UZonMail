﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Controllers.Statistics.Model;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Services.Settings;

namespace UZonMailService.Controllers.Statistics
{
    public class StatisticsController(SqlContext db, TokenService tokenService) : ControllerBaseV1
    {
        /// <summary>
        /// 获取发件箱统计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("outbox")]
        public async Task<ResponseResult<List<EmailCount>>> GetOutboxEmailCountInfo()
        {
            int userId = tokenService.GetIntUserId();
            var emailCounts = await db.Outboxes.OfType<Outbox>()
                .Where(x => x.UserId == userId)
                .Where(x => !x.IsDeleted)
                .GroupBy(x => x.Domain)
                .Select(x => new EmailCount
                {
                    Domain = x.Key,
                    Count = x.Count()
                })
                .ToListAsync();
            return emailCounts.ToSuccessResponse();
        }

        /// <summary>
        /// 获取收件箱统计信息
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("inbox")]
        public async Task<ResponseResult<List<EmailCount>>> GetInboxesEmailCountInfo()
        {
            int userId = tokenService.GetIntUserId();
            var emailCounts = await db.Inboxes
                .Where(x => x.BoxType == EmailBoxType.Inbox)
                .Where(x => x.UserId == userId)
                .Where(x => !x.IsDeleted)
                .GroupBy(x => x.Domain)
                .Select(x => new EmailCount
                {
                    Domain = x.Key,
                    Count = x.Count()
                })
                .ToListAsync();
            return emailCounts.ToSuccessResponse();
        }

        /// <summary>
        /// 每月发送邮件统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("monthly-sending")]
        public async Task<ResponseResult<List<MonthlySendingInfo>>> GetMonthlySendingCountInfo()
        {
            int userId = tokenService.GetIntUserId();
            var monthlySendingInfos = await db.SendingItems.OfType<SendingItem>()
                .Where(x => x.UserId == userId)
                .Where(x => !x.IsDeleted)
                .GroupBy(x => new { x.CreateDate.Year, x.CreateDate.Month })
                .Select(x => new MonthlySendingInfo
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Count = x.Count()
                })
                .ToListAsync();

            return monthlySendingInfos.ToSuccessResponse();
        }
    }
}
