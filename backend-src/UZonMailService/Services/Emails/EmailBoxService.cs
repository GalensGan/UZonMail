using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Services.Common;

namespace UZonMailService.Services.Emails
{
    /// <summary>
    /// 邮箱服务
    /// </summary>
    /// <param name="db"></param>
    public class EmailBoxService(SqlContext db) : CurdService<Inbox>(db)
    {
        /// <summary>
        /// 删除邮箱
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task DeleteEmailBox(Inbox inbox)
        {
            if (inbox == null) return;

            if (inbox.LinkCount > 0)
            {
                inbox.IsDeleted = true;
            }
            else
            {
                db.Inboxes.Remove(inbox);
            }

            await db.SaveChangesAsync();
        }
    }
}
