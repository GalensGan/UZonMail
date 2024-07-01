using System.Text.RegularExpressions;
using System.Timers;
using UZonMailService.Cache;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.EmailSending;
using UZonMailService.Models.SQL.Settings;
using UZonMailService.Services.EmailSending.Base;
using Timer = System.Timers.Timer;
using Uamazing.Utils.Extensions;
using System.Net.NetworkInformation;
using UZonMailService.Services.Settings;
using UZonMailService.Services.EmailSending.Event;
using UZonMailService.Services.EmailSending.Event.Commands;
using UZonMailService.Services.EmailSending.Models;

namespace UZonMailService.Services.EmailSending.OutboxPool
{
    /// <summary>
    /// 发件箱地址
    /// 该地址可能仅用于部分发件箱
    /// 也有可能是用于通用发件
    /// </summary>
    public class OutboxEmailAddress : EmailAddress, IDisposable, IDictionaryItem
    {
        #region 分布式锁
        public readonly object SendingItemIdsLock = new();
        #endregion

        #region 属性参数
        public OutboxEmailAddressType Type { get; private set; } = OutboxEmailAddressType.Specific;

        public Outbox _outbox;

        /// <summary>
        /// 当指定收件箱时，此处有值
        /// </summary>
        public HashSet<long> SendingItemIds { get; } = [];

        /// <summary>
        /// 用户 ID
        /// </summary>
        public long UserId => _outbox.UserId;

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight => _outbox.SendingWeight;

        /// <summary>
        /// 授权用户名
        /// </summary>
        public string? AuthUserName
        {
            get { return string.IsNullOrEmpty(_outbox.UserName) ? _outbox.Email : _outbox.UserName; }
        }

        /// <summary>
        /// 授权密码
        /// </summary>
        public string? AuthPassword { get; private set; }

        public string SmtpHost => _outbox.SmtpHost;

        public int SmtpPort => _outbox.SmtpPort;

        public bool EnableSSL => _outbox.EnableSSL;

        /// <summary>
        /// 单日最大发送数量
        /// 为 0 时表示不限制
        /// </summary>
        public int MaxSendCountPerDay => _outbox.MaxSendCountPerDay;

        private int _sentTotalToday;
        /// <summary>
        /// 当前已发送数量
        /// 成功失败都被计算在内
        /// </summary>
        public int SentTotalToday => _sentTotalToday;

        /// <summary>
        /// 代理 Id
        /// </summary>
        public long ProxyId => _outbox.ProxyId;

        /// <summary>
        /// 回复至邮箱
        /// </summary>
        public List<string> ReplyToEmails { get; set; } = [];
        /// <summary>
        /// 所属的发件箱组 id
        /// </summary>
        public HashSet<long> SendingGroupIds = [];

        /// <summary>
        /// 是否应释放
        /// </summary>
        public bool ShouldDispose { get; private set; } = false;

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable
        {
            get => !ShouldDispose && !_isCooldown;
        }

        /// <summary>
        /// key 是必须项
        /// </summary>
        public string Key => Email;

        /// <summary>
        /// 程序动态设置以下属性
        /// </summary>
        public double MinPercent { get; set; }
        public double MaxPercent { get; set; }
        #endregion

        #region 构造
        /// <summary>
        /// 生成发件地址
        /// </summary>
        /// <param name="outbox"></param>
        /// <param name="sendingGroupId"></param>
        /// <param name="authPassword"></param>
        public OutboxEmailAddress(Outbox outbox, long sendingGroupId, List<string> smtpPasswordSecretKeys, OutboxEmailAddressType type, List<long> sendingItemIds = null)
        {
            _outbox = outbox;
            AuthPassword = outbox.Password.DeAES(smtpPasswordSecretKeys.First(), smtpPasswordSecretKeys.Last());
            SendingGroupIds.Add(sendingGroupId);
            Type = type;
            if (sendingItemIds != null)
            {
                foreach (var id in sendingItemIds)
                {
                    SendingItemIds.Add(id);
                }
            }

            CreateDate = DateTime.Now;
            Email = outbox.Email;
            Name = outbox.Name;
            Id = outbox.Id;
            ReplyToEmails = outbox.ReplyToEmails.SplitBySeparators().Distinct().ToList();
            _sentTotalToday = outbox.SentTotalToday;
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新发件地址
        /// </summary>
        /// <param name="data"></param>
        public void Update(OutboxEmailAddress data)
        {
            // 更新发件组 id
            foreach (var id in data.SendingGroupIds)
            {
                SendingGroupIds.Add(id);
            }

            // 更新发送项 id
            foreach (var id in data.SendingItemIds)
            {
                SendingItemIds.Add(id);
            }

            // 更新类型
            Type |= data.Type;
        }
        #endregion

        #region 冷却状态切换
        private readonly object _lockObject = new();
        private DateTime _startDate = DateTime.Now;
        private bool _isCooldown = false;
        private Timer? _timer = null;
        /// <summary>
        /// 设置冷却
        /// 若设置失败，则返回 false
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetCooldown(ScopeServices scopeServices)
        {
            // 如果本身已经冷却，则不再冷却
            lock (_lockObject)
            {
                // 说明被其它线程已经使用了
                if (_isCooldown)
                {
                    return false;
                }
                _isCooldown = true;
            }

            // 启动 _timer 用于解除冷却
            // 计算随机值
            // 使用通用设置中的发送上限
            var settingReader = await UserSettingsCache.GetUserSettingsReader(scopeServices.SqlContext, UserId);
            int cooldownMilliseconds = settingReader.GetCooldownMilliseconds();
            if (cooldownMilliseconds <= 0)
            {
                _isCooldown = false;
                // 通知可以继续发件
                return true;
            }

            _timer?.Dispose();
            _timer = new Timer(cooldownMilliseconds)
            {
                AutoReset = false,
                Enabled = true
            };
            _timer.Elapsed += async (sender, args) =>
            {
                _timer.Stop();
                _isCooldown = false;
                // 通知可以继续发件
                await new StartSendingCommand(scopeServices, 1).Execute(this);
            };
            return true;
        }
        #endregion

        #region 外部调用方法
        /// <summary>
        /// 验证是否有效
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }

        /// <summary>
        /// 更新使用记录
        /// </summary>
        public async Task UpdateUsageInfo(ScopeServices scopeServices)
        {
            // 重置每日发件量
            if (_startDate.Date != DateTime.Now.Date)
            {
                _startDate = DateTime.Now;
                _sentTotalToday = 0;
            }
            else
            {
                // 防止并发增加
                Interlocked.Increment(ref _sentTotalToday);
            }

            var userSetting = await UserSettingsCache.GetUserSettingsReader(scopeServices.SqlContext, UserId);
            // 若已经达到发送上限，则不再发送
            if (MaxSendCountPerDay > 0)
            {
                if (SentTotalToday > MaxSendCountPerDay)
                {
                    ShouldDispose = true;
                    return;
                }
            }
            else if (userSetting.MaxSendCountPerEmailDay.Value > 0 && SentTotalToday >= userSetting.MaxSendCountPerEmailDay.Value)
            {
                ShouldDispose = true;
                return;
            }
        }
        #endregion
    }
}
