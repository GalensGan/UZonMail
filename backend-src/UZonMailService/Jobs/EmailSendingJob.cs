using Microsoft.EntityFrameworkCore;
using Quartz;
using UZonMail.Utils.Web.Service;
using UZonMailService.UzonMailDB.SQL;
using UZonMailService.UzonMailDB.SQL.EmailSending;
using UZonMailService.Services.EmailSending;

namespace UZonMailService.Jobs
{
    /// <summary>
    /// 邮件发送定时任务
    /// </summary>
    public class EmailSendingJob(SqlContext db, ILogger<EmailSendingJob> logger, SendingGroupService sendingService) : IJob, IScopedService
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("邮件发送定时任务开始执行");

            // 获取组 id
            var sendingGroupId = context.JobDetail.JobDataMap.GetInt("sendingGroupId");
            if (sendingGroupId == 0)
            {
                logger.LogError(" 邮件发送定时任务需传入发送组 id");
                return;
            }

            var sendingGroup = await db.SendingGroups.Where(x => x.Id == sendingGroupId
                    && x.Status == SendingGroupStatus.Scheduled
                    && x.SendingType == SendingGroupType.Scheduled)
                .Include(x => x.Outboxes)
                .Include(x => x.Templates)
                .FirstOrDefaultAsync();

            // 添加到立即发送队列
            if (sendingGroup == null) return;
            // 获取密钥
            string[] smtpPasswordSecretKeys = context.JobDetail.JobDataMap.GetString("smtpPasswordSecretKeys").Split(',');
            sendingGroup.SmtpPasswordSecretKeys = smtpPasswordSecretKeys.ToList();
            await sendingService.SendNow(sendingGroup);
        }
    }
}
