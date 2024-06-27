
using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using MimeKit;
using System.IO.Pipelines;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 本机发件
    /// </summary>
    public class LocalSender : SendMethod
    {
        private SendItem sendItem;
        public LocalSender(SendItem sendItem)
        {
            this.sendItem = sendItem;
        }
        /// <summary>
        /// 使用本机进行发件
        /// </summary>
        /// <returns></returns>
        public override async Task<SendResult> Send()
        {
            ArgumentNullException.ThrowIfNull(sendItem);

            if (!sendItem.Validate())
            {
                await UpdateSendingStatus(new SendResult(sendItem, false, "发件项数据不满足要求"));
                return new SendResult(sendItem, false, "发件项数据不满足要求") { SentStatus = SentStatus.Failed };
            }

            // 参考：https://github.com/jstedfast/MailKit/tree/master/Documentation/Examples
            // 本机发件逻辑
            var message = new MimeMessage();
            // 发件人
            message.From.Add(new MailboxAddress(sendItem.Outbox.Name, sendItem.Outbox.Email));
            // 收件人、抄送、密送           
            foreach (var address in sendItem.Inboxes)
            {
                if (string.IsNullOrEmpty(address.Email))
                    continue;
                message.To.Add(new MailboxAddress(address.Name, address.Email));
            }
            if (sendItem.CC != null)
                foreach (var address in sendItem.CC)
                {
                    if (string.IsNullOrEmpty(address.Email))
                        continue;
                    message.Cc.Add(new MailboxAddress(address.Name, address.Email));
                }
            if (sendItem.BCC != null)
                foreach (var address in sendItem.BCC)
                {
                    if (string.IsNullOrEmpty(address.Email))
                        continue;
                    message.Bcc.Add(new MailboxAddress(address.Name, address.Email));
                }
            // 回信人
            if (sendItem.ReplyToEmails.Count > 0)
            {
                message.ReplyTo.AddRange(sendItem.ReplyToEmails.Select(x =>
                {
                    return new MailboxAddress(x, x);
                }));
            }

            // 主题
            message.Subject = sendItem.GetSubject();
            // 正文
            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = sendItem.GetBody()
            };
            // 附件
            var attachments = await sendItem.GetAttachments();
            foreach (var attachment in attachments)
            {
                // 添加附件                
                bodyBuilder.Attachments.Add(attachment.Item1);
                // 修改文件名
                var lastOne = bodyBuilder.Attachments.Last();
                lastOne.ContentType.Name = attachment.Item2;
                lastOne.ContentDisposition.FileName = attachment.Item2;
            }
            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                var client = await SmtpClientFactory.GetSmtpClientAsync(sendItem.Outbox, sendItem.ProxyInfo);
                // 若返回 null,说明这个发件箱不能建立 smtp 连接，对它进行取消
                if (client == null)
                {
                    return new SendResult(sendItem, false, "发件项数据不满足要求") { SentStatus = SentStatus.Retry | SentStatus.OutboxConnectError };
                }

                //client.MessageSent += (sender, args) =>
                //{
                //    sendItem.Logger.LogInformation("Message sent: {0}", args.Message.Subject);
                //};
                //throw new NullReferenceException("测试报错");
#if DEBUG
                var sendResult = "debug success";
#else
                string sendResult = await client.SendAsync(message);
#endif
                var successResult = new SendResult(sendItem, true, sendResult);
                var status = await UpdateSendingStatus(successResult);
                successResult.SentStatus = status;
                return successResult;
            }
            catch (Exception ex)
            {
                sendItem.Logger.LogError(ex, ex.Message);
                var successResult = new SendResult(sendItem, true, ex.Message);
                var status = await UpdateSendingStatus(new SendResult(sendItem, false, ex.Message));
                successResult.SentStatus = status;
                return successResult;
            }
        }

        /// <summary>
        /// 发送完成后回调
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual async Task<SentStatus> UpdateSendingStatus(SendResult sendCompleteResult)
        {
            return await sendItem.UpdateSendingStatus(sendCompleteResult);
        }
    }
}
