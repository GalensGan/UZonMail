using System.ComponentModel.DataAnnotations.Schema;
using UZonMail.DB.SQL.Settings;
using UZonMail.Utils.Extensions;

namespace UZonMail.Core.Services.Settings
{
    /// <summary>
    /// 后者的值会覆盖前者的值
    /// </summary>
    /// <param name="settings"></param>
    public class UserSettingsReader(List<UserSetting> settings)
    {
        private List<UserSetting> _settings = settings.Where(x => x != null).OrderBy(x => x.Priority).ToList();

        private Lazy<int> GetIntSetting(Func<UserSetting, int> selector)
        {
            return new Lazy<int>(() =>
            {
                var values = _settings.Select(selector).Where(x => x > 0).ToList();
                return values.Count > 0 ? values.Last() : 0;
            });
        }
        private Lazy<string?> GetStringSetting(Func<UserSetting, string?> selector)
        {
            return new Lazy<string?>(() =>
            {
                var values = _settings.Select(selector).Where(x => !string.IsNullOrEmpty(x)).ToList();
                return values.Count > 0 ? values.Last() : "";
            });
        }

        /// <summary>
        /// 每日每个发件箱最大发送次数
        /// 为 0 时表示不限制
        /// </summary>
        public Lazy<int> MaxSendCountPerEmailDay => GetIntSetting(x => x.MaxSendCountPerEmailDay);

        /// <summary>
        /// 最小发件箱冷却时间
        /// </summary>
        public Lazy<int> MinOutboxCooldownSecond => GetIntSetting(x => x.MinOutboxCooldownSecond);

        /// <summary>
        /// 最大发件箱冷却时间
        /// </summary>
        public Lazy<int> MaxOutboxCooldownSecond => GetIntSetting(x => x.MaxOutboxCooldownSecond);

        /// <summary>
        /// 最大批量发件数
        /// </summary>
        public Lazy<int> MaxSendingBatchSize => GetIntSetting(x => x.MaxSendingBatchSize);

        /// <summary>
        /// 收件箱最小收件间隔时间，单位小时
        /// </summary>
        public Lazy<int> MinInboxCooldownHours => GetIntSetting(x => x.MinInboxCooldownHours);

        /// <summary>
        /// 回复的邮箱地址, 多个邮箱用逗号分隔
        /// </summary>
        public Lazy<string?> ReplyToEmails => GetStringSetting(x => x.ReplyToEmails);

        /// <summary>
        /// 回复邮件地址列表
        /// </summary>
        [NotMapped]
        public List<string> ReplyToEmailsList => ReplyToEmails.Value.SplitBySeparators().Distinct().ToList();

        #region 公开方法
        /// <summary>
        /// 获取冷却时间的毫秒数
        /// </summary>
        /// <returns></returns>
        public int GetCooldownMilliseconds()
        {
            if (MaxOutboxCooldownSecond.Value <= MinOutboxCooldownSecond.Value) return 0;
            var result = new Random().Next(MinOutboxCooldownSecond.Value, MaxOutboxCooldownSecond.Value) * 1000;
            return Math.Max(0, result);
        }
        #endregion
    }
}
