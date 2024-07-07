using log4net;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 具有速度限制的 smtp 客户端
    /// </summary>
    public class LimitedSmtpClient(string email, int cooldownMilliseconds) : SmtpClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LimitedSmtpClient));
        /// <summary>
        /// 至少间隔时间
        /// </summary>
        private static readonly int _minTimeIntervalMilliseconds = 500;

        private DateTime _lastDate = DateTime.Now;

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public override async Task<string> SendAsync(MimeMessage message, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            var now = DateTime.Now;
            var timeInverval = (int)(now - _lastDate).TotalMilliseconds;
            _lastDate = now;
            var controlValue = Math.Min(cooldownMilliseconds, _minTimeIntervalMilliseconds);
            if (timeInverval <= controlValue)
            {
                _logger.Warn($"{email} 发件间隔太短，将在 {timeInverval} 毫秒后开始发送");
                await Task.Delay(timeInverval);
            }

#if DEBUG
            return "send by debug";
#endif
            return await base.SendAsync(message, cancellationToken, progress);
        }
    }
}
