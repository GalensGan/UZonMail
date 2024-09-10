using log4net;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Collections.Concurrent;
using Uamazing.Utils.Environments;
using UZonMail.Core.Services.EmailSending.Base;
using UZonMail.Core.Services.EmailSending.OutboxPool;
using UZonMail.Core.Services.EmailSending.Pipeline;
using UZonMail.Core.Services.Settings;
using UZonMail.DB.SQL.Emails;

namespace UZonMail.Core.Services.EmailSending.Sender
{
    public class SmtpClientFactory
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SmtpClientFactory));
        private static ConcurrentDictionary<string, SmtpClient> _smptClients = new();

        /// <summary>
        /// 获取 smtp 客户端
        /// 有可能更换了账号密码，要重新获取
        /// </summary>
        /// <param name="outbox"></param>
        /// <returns></returns>
        public static async Task<FuncResult<SmtpClient>> GetSmtpClientAsync(SendingContext sendingContext, OutboxEmailAddress outbox, ProxyInfo? proxyInfo)
        {
            var key = outbox.Email;
            if (_smptClients.TryGetValue(key, out var value))
            {
                // 判断是否过期
                if (value.IsConnected) return new FuncResult<SmtpClient>() { Data = value };
                // 说明已经断开,进行移除
                await value.DisconnectAsync(true);
            }

            _logger.Info($"初始化 SmtpClient: {outbox.AuthUserName}");
            var settingReader = await UserSettingsCache.GetUserSettingsReader(sendingContext.SqlContext, outbox.UserId);
            int cooldownMilliseconds = settingReader.MinOutboxCooldownSecond.Value;
            var client = new LimitedSmtpClient(outbox.Email, cooldownMilliseconds);
            try
            {
                if (proxyInfo != null)
                {
                    client.ProxyClient = proxyInfo.GetProxyClient(_logger);
                }

                // 对证书过期进行兼容处理
                try
                {                    
                    client.Connect(outbox.SmtpHost, outbox.SmtpPort, outbox.EnableSSL ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);
                }
                catch (SslHandshakeException ex)
                {
                    _logger.Warn(ex);
                    // 证书过期
                    if (!outbox.Enable)
                    {
                        client.Connect(outbox.SmtpHost, outbox.SmtpPort, SecureSocketOptions.None);
                    }
                }

                // Note: only needed if the SMTP server requires authentication
                // 进行鉴权
                if (!Env.IsDebug)
                    if (!string.IsNullOrEmpty(outbox.AuthPassword)) client.Authenticate(outbox.AuthUserName, outbox.AuthPassword);
                _smptClients.TryAdd(key, client);
                return new FuncResult<SmtpClient>() { Data = client };
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                client.Disconnect(true);
                client.Dispose();
                return new FuncResult<SmtpClient>()
                {
                    Ok = false,
                    Message = ex.Message,
                    Data = null,
                };
            }
        }
    }
}
