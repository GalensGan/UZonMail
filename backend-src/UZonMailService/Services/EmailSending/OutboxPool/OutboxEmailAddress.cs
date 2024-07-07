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
using System.Collections.Concurrent;
using UZonMailService.Services.EmailSending.Pipeline;
using UZonMailService.Utils.Database;
using UZonMailService.Services.EmailSending.Sender;
using log4net;

namespace UZonMailService.Services.EmailSending.OutboxPool
{
    /// <summary>
    /// 发件箱地址
    /// 该地址可能仅用于部分发件箱
    /// 也有可能是用于通用发件
    /// </summary>
    public class OutboxEmailAddress : EmailAddress, IDisposable, IWeight
    {
        #region 分布式锁
        public readonly object SendingItemIdsLock = new();
        #endregion

        private readonly static ILog _logger = LogManager.GetLogger(typeof(OutboxEmailAddress));

        #region 属性参数
        public OutboxEmailAddressType Type { get; private set; } = OutboxEmailAddressType.Specific;

        public Outbox _outbox;

        /// <summary>
        /// 当指定收件箱时，此处有值
        /// </summary>
        public ConcurrentQueue<long> SendingItemIds { get; } = [];

        /// <summary>
        /// 用户 ID
        /// </summary>
        public long UserId => _outbox.UserId;

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; private set; }

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
            get => !ShouldDispose && !_isCooldown && !_isUsing;
        }
        #endregion

        #region 构造
        /// <summary>
        /// 生成发件地址
        /// </summary>
        /// <param name="outbox"></param>
        /// <param name="sendingGroupId"></param>
        /// <param name="smtpPasswordSecretKeys"></param>
        /// <param name="type"></param>
        /// <param name="sendingItemIds"></param>
        public OutboxEmailAddress(Outbox outbox, long sendingGroupId, List<string> smtpPasswordSecretKeys, OutboxEmailAddressType type, List<long> sendingItemIds = null)
        {
            _outbox = outbox;
            AuthPassword = outbox.Password.DeAES(smtpPasswordSecretKeys.First(), smtpPasswordSecretKeys.Last());
            SendingGroupIds.Add(sendingGroupId);
            Type = type;
            if (sendingItemIds != null)
            {
                foreach (var id in sendingItemIds.Distinct())
                {
                    SendingItemIds.Enqueue(id);
                }
            }

            CreateDate = DateTime.Now;
            Email = outbox.Email;
            Name = outbox.Name;
            Id = outbox.Id;
            ReplyToEmails = outbox.ReplyToEmails.SplitBySeparators().Distinct().ToList();
            _sentTotalToday = outbox.SentTotalToday;

            Weight = outbox.Weight > 0 ? outbox.Weight : 1;
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新发件地址
        /// 非并发操作
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
            var existIds = data.SendingItemIds.Except(SendingItemIds).ToList();
            existIds.ForEach(x => SendingItemIds.Enqueue(x));

            // 更新类型
            Type |= data.Type;
        }
        #endregion

        #region 使用和冷却状态切换
        private readonly object _usingLock = new();
        private bool _isUsing = false;

        private DateTime _startDate = DateTime.Now;
        private bool _isCooldown = false;
        private Timer? _timer = null;

        /// <summary>
        /// 锁定使用权
        /// </summary>
        /// <returns></returns>
        public bool LockUsing()
        {
            lock (_usingLock)
            {
                if (_isUsing)
                {
                    return false;
                }
                _isUsing = true;
                return true;
            }
        }

        /// <summary>
        /// 释放使用权
        /// 需要在程序逻辑最后一刻才释放
        /// </summary>
        public void UnlockUsing()
        {
            lock (_usingLock)
            {
                _isUsing = false;
            }
        }

        /// <summary>
        /// 设置冷却
        /// 若设置失败，则返回 false
        /// </summary>
        /// <returns></returns>
        public async Task SetCooldown(SendingContext sendingContext)
        {
            // 说明被其它线程已经使用了
            if (_isCooldown)
                return;

            _isCooldown = true;

            // 启动 _timer 用于解除冷却
            // 计算随机值
            // 使用通用设置中的发送上限
            var settingReader = await UserSettingsCache.GetUserSettingsReader(sendingContext.SqlContext, UserId);
            int cooldownMilliseconds = settingReader.GetCooldownMilliseconds();
            if (cooldownMilliseconds <= 0)
            {
                _isCooldown = false;
                // 通知可以继续发件
                return;
            }

            _logger.Debug($"发件箱 {Email} 进入冷却状态，冷却时间 {cooldownMilliseconds} 毫秒");
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
                _logger.Debug($"发件箱 {Email} 退出冷却状态");
                // 通知可以继续发件
                await new StartSendingCommand(1).Execute(this);
            };
            return;
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
        public async Task EmailItemSendCompleted(SendingContext sendingContext)
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

            var userSetting = await UserSettingsCache.GetUserSettingsReader(sendingContext.SqlContext, UserId);
            // 本身有限制时，若已经达到发送上限，则不再发送
            if (MaxSendCountPerDay > 0)
            {
                if (SentTotalToday > MaxSendCountPerDay)
                {
                    ShouldDispose = true;
                }
            }
            // 本身没限制，使用系统的限制
            else if (userSetting.MaxSendCountPerEmailDay.Value > 0 && SentTotalToday >= userSetting.MaxSendCountPerEmailDay.Value)
            {
                ShouldDispose = true;
            }

            // 若是发件连接失败，则移除
            if (sendingContext.SendResult.SentStatus.HasFlag(SentStatus.OutboxConnectError) 
                || sendingContext.SendResult.SentStatus.HasFlag(SentStatus.EmptySendingGroup))
            {
                ShouldDispose = true;
            }

            // 判断发件箱是否需要释放
            if (sendingContext.OutboxEmailAddress.ShouldDispose)
            {
                // 受影响的发件任务
                var sendingGroupIds = sendingContext.OutboxEmailAddress.SendingGroupIds;
                // 移除指定发件箱的发件项
                foreach (var sendingGroupId in sendingGroupIds)
                {
                    if (!sendingContext.UserSendingGroupsPool.TryGetValue(sendingGroupId, out var sendingGroup)) continue;

                    // 判断当前发件组是否还有发件箱
                    var existOtherOutbox = sendingContext.UserOutboxesPool.Values.Any(x => x.Email != sendingContext.OutboxEmailAddress.Email
                        && x.SendingGroupIds.Contains(sendingGroupId));
                    if (!existOtherOutbox)
                    {
                        // 移除发件组
                        var removeGroupResult = await sendingContext.UserSendingGroupsPool.TryRemoveSendingGroupTask(sendingContext, sendingGroupId);
                        if (removeGroupResult)
                        {
                            // 修改发件项状态
                            await sendingContext.SqlContext.SendingItems.UpdateAsync(x => x.SendingGroupId == sendingGroupId && x.Status == SendingItemStatus.Pending
                            , x => x.SetProperty(y => y.Status, SendingItemStatus.Failed)
                                .SetProperty(y => y.SendResult, sendingContext.SendResult.Message ?? "发件箱退出发件池，无发件箱可用")
                            );
                            // 修改发件组状态
                            await sendingContext.SqlContext.SendingGroups.UpdateAsync(x => x.Id == sendingGroupId, x => x.SetProperty(y => y.Status, SendingGroupStatus.Finish));

                            // 通知发件组发送完成
                            await removeGroupResult.Data.NotifyEnd(sendingContext, sendingGroupId);
                        }

                        continue;
                    }

                    // 移除指定发件箱的发件项
                    await sendingGroup.RemoveSpecificSendingItems(sendingContext);
                }
            }

            // 向上继续调用
            await sendingContext.UserOutboxesPool.EmailItemSendCompleted(sendingContext);
        }
        #endregion
    }
}
