using Microsoft.EntityFrameworkCore;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.Settings;

namespace UZonMailService.Services.Settings
{
    /// <summary>
    /// 设置
    /// 传入的值一定不能为 null
    /// </summary>
    public class UserSettingsFactory
    {
        /// <summary>
        /// 创建用户设置
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<UserSetting> GetUserSettings(SqlContext db, int userId)
        {            
            var settings = await db.UserSettings.Where(x => x.UserId==userId).FirstOrDefaultAsync();
            if(settings == null)
            {
                settings = new UserSetting
                {
                    UserId = userId
                };
                db.UserSettings.Add(settings);
            }
            if (settings == null)
                await db.SaveChangesAsync();

            return settings;
        }
    }
}
