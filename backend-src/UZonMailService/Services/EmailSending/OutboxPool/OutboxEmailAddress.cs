using System.Timers;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.EmailSending;
using UZonMailService.Models.SQL.Settings;
using Timer = System.Timers.Timer;

namespace UZonMailService.Services.EmailSending.OutboxPool
{
    /// <summary>
    /// 发件箱地址
    /// 与 Outbox 对应，不使用 outbox 的原因是防止 Outbox 上下文被释放后，操作 Outbox 出错
    /// </summary>
    public class OutboxEmailAddress : EmailAddress, IDisposable
    {
        #region 所属分类
        /// <summary>
        /// 所属的发件箱组 id
        /// </summary>
        public HashSet<int> SendingGroupIds = [];
        #endregion

        #region 构造
        private UserSetting _userSetting;

        /// <summary>
        /// 初始化发件地址
        /// </summary>
        /// <param name="userSetting"></param>
        public OutboxEmailAddress(UserSetting userSetting)
        {
            _userSetting = userSetting;
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新发件地址
        /// </summary>
        /// <param name="data"></param>
        public void Update(OutboxEmailAddress data)
        {
            _userSetting = data._userSetting;
            foreach (var id in data.SendingGroupIds)
            {
                SendingGroupIds.Add(id);
            }
        }
        #endregion

        #region 设置
        private string? _authUserName;
        /// <summary>
        /// 授权用户名
        /// </summary>
        public string? AuthUserName
        {
            get { return string.IsNullOrEmpty(_authUserName) ? Email : _authUserName; }
            set { _authUserName = value; }
        }

        /// <summary>
        /// 授权密码
        /// </summary>
        public string? AuthPassword { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public bool EnableSSL { get; set; }

        /// <summary>
        /// 单日最大发送数量
        /// 为 0 时表示不限制
        /// </summary>
        public int MaxSendCountPerDay { get; set; }

        /// <summary>
        /// 当前已发送数量
        /// 成功失败都被计算在内
        /// </summary>
        public int SentTotalToday { get; set; }

        /// <summary>
        /// 代理 Id
        /// </summary>
        public int ProxyId { get; set; }

        public bool ShouldDispose { get; private set; } = false;
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable
        {
            get => !ShouldDispose && !_isCooldown;
        }
        #endregion

        #region 状态
        private DateTime _startDate = DateTime.Now;
        private bool _isCooldown = false;
        private Timer _timer = null;
        /// <summary>
        /// 设置冷却
        /// </summary>
        /// <returns></returns>
        public void SetCooldown(Action<int> finish)
        {
            _isCooldown = true;

            // 每日重置
            if(_startDate.Date != DateTime.Now.Date)
            {
                _startDate = DateTime.Now;
                SentTotalToday = 0;
            }

            // 若已经达到发送上限，则不再发送
            SentTotalToday++;
            // 使用发件箱自定义的发送上限
            if (MaxSendCountPerDay > 0)
            {
                if (SentTotalToday > MaxSendCountPerDay)
                {
                    ShouldDispose = true;
                    return;
                }
            }
            // 使用通用设置中的发送上限
            else if (_userSetting.MaxSendCountPerEmailDay > 0 && SentTotalToday > _userSetting.MaxSendCountPerEmailDay)
            {
                ShouldDispose = true;
                return;
            }


            // 启动 _timer 用于解除冷却
            // 计算随机值
            int cooldownMilliseconds = _userSetting.GetCooldownMilliseconds();
            if (cooldownMilliseconds <= 0)
            {
                _isCooldown = false;
                // 通知可以继续发件
                finish?.Invoke(1);
                return;
            }

            _timer?.Dispose();
            _timer = new Timer(cooldownMilliseconds)
            {
                AutoReset = false,
                Enabled = true
            };
            _timer.Elapsed += (sender, args) =>
            {
                _timer.Stop();
                _isCooldown = false;
                // 通知可以继续发件
                finish?.Invoke(1);
            };
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
