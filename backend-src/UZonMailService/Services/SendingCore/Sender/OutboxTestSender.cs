using log4net;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.IO.Pipelines;
using UZonMail.Utils.Extensions;
using UZonMail.Utils.Results;
using UZonMailService.Controllers.Users.Model;
using UZonMailService.UzonMailDB.SQL;
using UZonMailService.UzonMailDB.SQL.Emails;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 发件箱测试
    /// </summary>
    /// <param name="outbox"></param>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="outbox"></param>
    /// <param name="smtpPasswordSecretKeys"></param>
    /// <param name="sqlContext"></param>
    public class OutboxTestSender(Outbox outbox, SmtpPasswordSecretKeys smtpPasswordSecretKeys, SqlContext sqlContext)
    {
        private readonly static ILog _logger = LogManager.GetLogger(typeof(OutboxTestSender));

        /// <summary>
        /// 发送测试
        /// </summary>
        /// <returns></returns>
        public async Task<Result<string>> SendTest()
        {
            // 参考：https://github.com/jstedfast/MailKit/tree/master/Documentation/Examples
            // 本机发件逻辑
            var message = new MimeMessage();
            var mailbox = new MailboxAddress(outbox.Name, outbox.Email);
            // 发件人
            message.From.Add(mailbox);
            // 主题
            message.Subject = "SMTP Testing";
            message.To.Add(mailbox);
            // 正文
            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = "This email is for SMTP Testing"
            };
            message.Body = bodyBuilder.ToMessageBody();
            try
            {
                var client = new SmtpClient();
                // 获取代理
                if (outbox.ProxyId > 0)
                {
                    var proxy = await sqlContext.UserProxies.Where(x => x.UserId == outbox.UserId || x.IsShared)
                        .Where(x => x.Id == outbox.ProxyId)
                        .FirstOrDefaultAsync();
                    if (proxy != null)
                        client.ProxyClient = proxy.ToProxyInfo().GetProxyClient(_logger);
                }
                client.Connect(outbox.SmtpHost, outbox.SmtpPort, outbox.EnableSSL);
                // 鉴权
                if (!string.IsNullOrEmpty(outbox.Password))
                {
                    var password = outbox.Password.DeAES(smtpPasswordSecretKeys.Key, smtpPasswordSecretKeys.Iv);
                    client.Authenticate(string.IsNullOrEmpty(outbox.UserName) ? outbox.Email : outbox.UserName, password);
                }

                string sendResult = await client.SendAsync(message);
                return new Result<string>(true, sendResult);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                return new Result<string>(false, ex.Message, ex.Message);
            }
        }
    }
}
