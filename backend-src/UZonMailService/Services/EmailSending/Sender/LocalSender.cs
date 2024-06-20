
using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using MimeKit;

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
        public override async Task<SentStatus> Send()
        {
            ArgumentNullException.ThrowIfNull(sendItem);

            if (!sendItem.Validate())
            {
                await UpdateSendingStatus(false, "发件项数据不满足要求");
                return SentStatus.Failed;
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

            using var client = new SmtpClient();
            try
            {
                // 添加代理
                if (sendItem.ProxyInfo != null)
                {
                    client.ProxyClient = sendItem.ProxyInfo?.GetProxyClient(sendItem.Logger);
                }
                client.Connect(sendItem.Outbox.SmtpHost, sendItem.Outbox.SmtpPort, sendItem.Outbox.EnableSSL);

                // Note: only needed if the SMTP server requires authentication
                // 进行鉴权
                if (!string.IsNullOrEmpty(sendItem.Outbox.AuthPassword)) client.Authenticate(sendItem.Outbox.AuthUserName, sendItem.Outbox.AuthPassword);

                //client.MessageSent += (sender, args) =>
                //{
                //    sendItem.Logger.LogInformation("Message sent: {0}", args.Message.Subject);
                //};
                //throw new NullReferenceException("测试报错");
                string sendResult = await client.SendAsync(message);
                //var sendResult = "test";
                return await UpdateSendingStatus(true, sendResult);
            }
            catch (Exception ex)
            {
                sendItem.Logger.LogError(ex, ex.Message);
                return await UpdateSendingStatus(false, ex.Message);
            }
            finally
            {
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// 发送完成后回调
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual async Task<SentStatus> UpdateSendingStatus(bool ok, string message)
        {
            return await sendItem.UpdateSendingStatus(ok, message);
        }
    }
}
