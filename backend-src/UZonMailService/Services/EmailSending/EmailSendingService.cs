using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.EmailSending.WaitList;

namespace UZonMailService.Services.EmailSending
{
    /// <summary>
    /// 为 EmailSendingController 提供服务
    /// </summary>
    public class EmailSendingService(SqlContext db
        , SystemTasksService tasksService
        , SystemSendingWaitListService waitList) : IScopedService
    {
        /// <summary>
        /// 恢复中断的发件任务
        /// </summary>
        /// <returns></returns>
        public async Task RecoverSending()
        {
            var sendingGroups = await db.SendingGroups.Where(x => x.Status == SendingGroupStatus.Sending).ToListAsync();
            foreach (var item in sendingGroups)
            {
                await waitList.AddSendingGroup(item);
            }

            if (sendingGroups.Count > 0)
            {
                tasksService.StartSending();
            }
        }
    }
}
