using Microsoft.EntityFrameworkCore;
using UZonMail.Core.Services.EmailSending.Sender;
using UZonMail.Core.Services.SendCore.Contexts;
using UZonMail.Core.Services.SendCore.WaitList;
using UZonMail.Core.SignalRHubs.Extensions;
using UZonMail.Core.SignalRHubs.SendEmail;
using UZonMail.Core.Utils.Database;
using UZonMail.DB.SQL.Emails;
using UZonMail.DB.SQL.EmailSending;

namespace UZonMail.Core.Services.SendCore.ResponsibilityChains
{
    /// <summary>
    /// 发件项后处理器
    /// 1. 更新发件状态到数据库
    /// 2. 判断是否可以重试
    /// 3. 发送通知
    /// </summary>
    public class EmailItemPostHandler : AbstractSendingHandler
    {
        protected override async Task HandleCore(SendingContext context)
        {
            // 发送发件进度
            if (context.EmailItem == null) return;

            // 根据状态发送进度信息
            var emailItem = context.EmailItem;

            // 判断发件项是否需要重试


            var client = context.HubClient.GetUserClient(context.OutboxAddress.UserId);
            if (emailItem.Status.HasFlag(SendItemMetaStatus.Success) || emailItem.Status.HasFlag(SendItemMetaStatus.Error))
            {
                var sendingItem = await SaveSendingItemInfos(context);
                // 通知前端
                await client.SendingItemStatusChanged(new SendingItemStatusChangedArg(sendingItem));
            }

            // 增加重试次数
            emailItem.IncreaseTriedCount();

            var sendCompleteResult = sendingContext.SendResult;
            // 判断是否需要重试，满足以下条件则重试
            // 1. 状态码非 ForbiddenRetring
            // 2. 重试次数未达到上限
            // 3. 非指定发件箱
            if (!sendCompleteResult.Ok
                && !sendCompleteResult.SentStatus.HasFlag(SentStatus.ForbiddenRetring)
                && SendItemMeta.TriedCount < MaxRetryCount
                && string.IsNullOrEmpty(SendItemMeta.OutboxEmail))
            {
                // TODO: 使用其它发件箱重试
                if (sendCompleteResult.SentStatus.HasFlag(SentStatus.OutboxError))
                {
                    // 不再使用当前发件箱发件
                    // 向发件箱中添加标记
                    // 在取件时，根据标记过滤这些发件箱
                }

                // 通知上层并返回重试状态
                sendCompleteResult.SentStatus |= SentStatus.Retry;
            }
            else
            {

                // 保存到数据库
                SendingItem updatedItem = await SaveSendItemInfos(sendingContext);

                // 通知发送结果
                var client = sendingContext.HubClient.GetUserClient(SendingItem.UserId);
                if (client != null)
                {
                    await client.SendingItemStatusChanged(new SendingItemStatusChangedArg(updatedItem));
                }
            }

            // 调用回调,通知上层处理结果
            await sendingContext.SendingGroupTask.EmailItemSendCompleted(sendingContext);
        }

        /// <summary>
        /// 保存 SendItem 状态
        /// </summary>
        /// <returns></returns>
        private async Task<SendingItem> SaveSendingItemInfos(SendingContext sendingContext)
        {
            var outbox = sendingContext.OutboxAddress;
            var emailItem = sendingContext.EmailItem;

            var db = sendingContext.SqlContext;

            var success = emailItem.Status.HasFlag(SendItemMetaStatus.Success);
            var message = emailItem.Message;

            // 更新 sendingItems 状态            
            var data = await db.SendingItems.FirstOrDefaultAsync(x => x.Id == emailItem.SendingItemId);
            // 更新数据
            data.FromEmail = outbox.Email;
            data.Subject = emailItem.Subject;
            data.Content = emailItem.HtmlBody;
            // 保存发送状态
            data.Status = success ? SendingItemStatus.Success : SendingItemStatus.Failed;
            data.SendResult = message;
            data.TriedCount = emailItem.TriedCount;
            data.SendDate = DateTime.Now;
            // 解析邮件 id
            data.ReceiptId = new ResultParser(message).GetReceiptId();

            // 更新 sendingItemInbox 状态
            await db.SendingItemInboxes.UpdateAsync(x => x.SendingItemId == emailItem.SendingItemId,
                x => x.SetProperty(y => y.FromEmail, outbox.Email)
                    .SetProperty(y => y.SendDate, DateTime.Now)
                );

            // 更新收件箱的最近收件日期
            await db.Inboxes.UpdateAsync(x => emailItem.Inboxes.Select(x => x.Id).Contains(x.Id),
                               x => x.SetProperty(y => y.LastBeDeliveredDate, DateTime.Now)
                                .SetProperty(y => y.LastSuccessDeliveryDate, DateTime.Now));

            await db.SaveChangesAsync();

            return data;
        }
    }
}
