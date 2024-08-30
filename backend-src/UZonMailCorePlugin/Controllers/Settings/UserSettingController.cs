using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UZonMail.Utils.Web.Extensions;
using UZonMail.Utils.Web.ResponseModel;
using UZonMail.Core.Services.Settings;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Settings;

namespace UZonMail.Core.Controllers.Settings
{
    /// <summary>
    /// 用户设置控制器
    /// </summary>
    /// <param name="db"></param>
    /// <param name="tokenService"></param>
    public class UserSettingController(SqlContext db,TokenService tokenService) : ControllerBaseV1
    {
        /// <summary>
        /// 获取用户的设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult<UserSetting>> GetUserSettings()
        {
            var userId = tokenService.GetUserDataId();
            UserSetting? userSetting = await db.UserSettings.OfType<UserSetting>().FirstOrDefaultAsync(x => x.UserId == userId);
            if (userSetting == null)
            {
                userSetting = new UserSetting
                {
                    UserId = userId                   
                };
                db.UserSettings.Add(userSetting);
                await db.SaveChangesAsync();
            }

            return userSetting.ToSuccessResponse();
        }

        /// <summary>
        /// 更新用户设置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseResult<bool>> UpsertUserSetting([FromBody] UserSetting userSetting)
        {
            var userId = tokenService.GetUserDataId();
            UserSetting? exist = await db.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId);
            if (exist == null)
            {
                userSetting.UserId = userId;
                db.UserSettings.Add(userSetting);               
            }
            else
            {
                // 说明存在，更新
                exist.MaxSendCountPerEmailDay = userSetting.MaxSendCountPerEmailDay;
                exist.MaxOutboxCooldownSecond = userSetting.MaxOutboxCooldownSecond;
                exist.MinOutboxCooldownSecond = userSetting.MinOutboxCooldownSecond;
                exist.MaxSendingBatchSize = userSetting.MaxSendingBatchSize;
                exist.MinInboxCooldownHours = userSetting.MinInboxCooldownHours;
                exist.ReplyToEmails = userSetting.ReplyToEmails;
            }
            await db.SaveChangesAsync();

            // 更新到缓存
            UserSettingsCache.UpdateUserSettings(userId);

            return true.ToSuccessResponse();
        }
    }
}
