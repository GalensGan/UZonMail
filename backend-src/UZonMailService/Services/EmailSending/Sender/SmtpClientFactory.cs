using log4net;
using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using System.Collections.Concurrent;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Services.EmailSending.OutboxPool;

namespace UZonMailService.Services.EmailSending.Sender
{
    public class SmtpClientFactory
    {
        private static ILog _logger = LogManager.GetLogger(typeof(SmtpClientFactory));
        private static ConcurrentDictionary<string, SmtpClient> _smptClients = new ConcurrentDictionary<string, SmtpClient>();

        /// <summary>
        /// 获取 smtp 客户端
        /// 有可能更换了账号密码，要重新获取
        /// </summary>
        /// <param name="outbox"></param>
        /// <returns></returns>
        public static async Task<SmtpClient?> GetSmtpClientAsync(OutboxEmailAddress outbox, ProxyInfo? proxyInfo)
        {
            var key = outbox.AuthUserName;
            if(_smptClients.TryGetValue(key,out var value))
            {
                _logger.Info($"获取 SmtpClient: {outbox.AuthUserName}");
                // 判断是否过期
                if (value.IsConnected) return value;
                // 说明已经断开,进行移除
                await value.DisconnectAsync(true);
            }

            _logger.Info($"初始化 SmtpClient: {outbox.AuthUserName}");
            var client = new SmtpClient();
            try
            {
                if (proxyInfo != null)
                {
                    client.ProxyClient = proxyInfo.GetProxyClient(_logger);
                }

                client.Connect(outbox.SmtpHost, outbox.SmtpPort, outbox.EnableSSL);
                // Note: only needed if the SMTP server requires authentication
                // 进行鉴权
                if (!string.IsNullOrEmpty(outbox.AuthPassword)) client.Authenticate(outbox.AuthUserName, outbox.AuthPassword);

                _smptClients.TryAdd(key, client);
                return client;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                client.Disconnect(true);
                client.Dispose();
                return null;
            }
        }
    }
}
