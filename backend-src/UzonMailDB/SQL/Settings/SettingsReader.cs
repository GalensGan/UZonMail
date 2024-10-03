using System.ComponentModel.DataAnnotations.Schema;
using UZonMail.DB.Extensions;
using UZonMail.DB.SQL.Settings;

namespace UZonMail.DB.SQL.Settings
{
    /// <summary>
    /// 后者的值会覆盖前者的值
    /// </summary>
    /// <param name="settings"></param>
    public class SettingsReader(long key)
    {
        /// <summary>
        /// 键值
        /// </summary>
        private List<string> _keys = [key.ToString()];

        /// <summary>
        /// 系统设置
        /// </summary>
        private List<OrganizationSetting> _orgSettings = [];

        private Lazy<int> GetIntSetting(Func<OrganizationSetting, int> selector)
        {
            return new Lazy<int>(() =>
            {
                var values = _orgSettings.Select(selector).Where(x => x > 0).ToList();
                return values.Count > 0 ? values.Last() : 0;
            });
        }
        private Lazy<string?> GetStringSetting(Func<OrganizationSetting, string?> selector)
        {
            return new Lazy<string?>(() =>
            {
                var values = _orgSettings.Select(selector).Where(x => !string.IsNullOrEmpty(x)).ToList();
                return values.Count > 0 ? values.Last() : "";
            });
        }

        /// <summary>
        /// 获取 bool 设置
        /// 为空时表示不设置
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        private Lazy<bool> GetBoolSetting(Func<OrganizationSetting, bool?> selector)
        {
            return new Lazy<bool>(() =>
            {
                var lastValue = _orgSettings.Select(selector)
                .Where(x => x != null)
                .LastOrDefault();
                return lastValue ?? false;
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

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public Lazy<int> MaxRetryCount => GetIntSetting(x => x.MaxRetryCount);

        /// <summary>
        /// 是否允许发送邮件跟踪器
        /// </summary>
        public Lazy<bool> EnableEmailTracker => GetBoolSetting(x => x.EnableEmailTracker);

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

        /// <summary>
        /// 添加设置
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool AddSetting(OrganizationSetting? setting)
        {
            if (setting == null) return false;
            _orgSettings.Add(setting);
            _orgSettings = [.. _orgSettings.OrderBy(x => x.Priority)];
            return true;
        }

        public bool IsMatch(string key)
        {
            return _keys.Contains(key);
        }

        public bool IsMatch(long key)
        {
            return IsMatch(key.ToString());
        }
        #endregion
    }
}
