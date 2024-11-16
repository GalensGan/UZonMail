﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.ResponseModel;
using UZonMail.Core.Services.Permission;
using UZonMail.Core.Services.Settings;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Settings;
using UZonMail.Managers.Cache;
using UZonMail.Utils.Web.ResponseModel;

namespace UZonMail.Core.Controllers.Settings
{
    /// <summary>
    /// 组织设置
    /// </summary>
    public class OrganizationSettingController(SqlContext db, TokenService tokenService, PermissionService permissionService) : ControllerBaseV1
    {
        /// <summary>
        /// 获取组织的设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult<OrganizationSetting>> GetUserSettings()
        {
            var userId = tokenService.GetUserDataId();
            var organizationId = tokenService.GetOrganizationId();

            var orgSetting = await db.OrganizationSettings.Where(x => x.OrganizationId == organizationId).FirstOrDefaultAsync();
            if (orgSetting == null)
            {
                orgSetting = new OrganizationSetting
                {
                    OrganizationId = organizationId
                };
                db.Add(orgSetting);
                await db.SaveChangesAsync();
            }

            return orgSetting.ToSuccessResponse();
        }

        /// <summary>
        /// 更新组织设置
        /// 只有组织管理员可以更新这个设置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseResult<bool>> UpsertOrganizationSetting([FromBody] OrganizationSetting organizationSettings)
        {
            var userId = tokenService.GetUserDataId();
            // 判断当前用户是否是组织管理员
            var isOrganization = await permissionService.HasOrganizationPermission(userId);
            if (!isOrganization)
            {
                return false.ToFailResponse("You are not organization admin");
            }

            var organizationId = tokenService.GetOrganizationId();
            OrganizationSetting? exist = await db.OrganizationSettings.FirstOrDefaultAsync(x => x.OrganizationId == organizationId);
            if (exist == null)
            {
                organizationSettings.OrganizationId = organizationId;
                db.OrganizationSettings.Add(organizationSettings);
                exist = organizationSettings;
            }
            else
            {
                // 说明存在，更新
                exist.MaxSendCountPerEmailDay = organizationSettings.MaxSendCountPerEmailDay;
                exist.MaxOutboxCooldownSecond = organizationSettings.MaxOutboxCooldownSecond;
                exist.MinOutboxCooldownSecond = organizationSettings.MinOutboxCooldownSecond;
                exist.MaxSendingBatchSize = organizationSettings.MaxSendingBatchSize;
                exist.MinInboxCooldownHours = organizationSettings.MinInboxCooldownHours;
                exist.ReplyToEmails = organizationSettings.ReplyToEmails;
                exist.EnableEmailTracker = organizationSettings.EnableEmailTracker;
            }
            await db.SaveChangesAsync();

            // 更新到缓存
            var organization = await db.Departments.FirstOrDefaultAsync(x => x.Id == exist.OrganizationId);
            CacheManager.UpdateCache<OrganizationSettingCache>(organization.ObjectId);

            return true.ToSuccessResponse();
        }
    }
}
