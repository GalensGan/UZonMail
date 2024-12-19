using UZonMail.Core.Services.EmailSending.Base;
using Timer = System.Timers.Timer;
using UZonMail.Core.Services.EmailSending.Event.Commands;
using System.Collections.Concurrent;
using UZonMail.Core.Services.EmailSending.Pipeline;
using UZonMail.Core.Services.EmailSending.Sender;
using UZonMail.Utils.Extensions;
using UZonMail.Core.Utils.Database;
using log4net;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.DB.SQL.Emails;
using UZonMail.Core.Services.SendCore.Utils;
using UZonMail.Core.Services.SendCore.Interfaces;
using UZonMail.DB.Managers.Cache;

namespace UZonMail.Core.Services.SendCore.Outboxes
{
    /// <summary>
    /// 发件箱地址
    /// 该地址可能仅用于部分发件箱
    /// 也有可能是用于通用发件
    /// </summary>
    public class OutboxEmailAddress : EmailAddress, IWeight
    {
        private readonly static ILog _logger = LogManager.GetLogger(typeof(OutboxEmailAddress));

        #region 私有变量
        private readonly object _lock = new();

        // 发件箱数据
        private Outbox _outbox;

        /// <summary>
        /// 当指定邮件被指定收件箱时，此处有值
        /// </summary>
        private HashSet<long> _sendingItemIds { get; } = [];

        /// <summary>
        /// 所属的发件箱组 id
        /// </summary>
        private HashSet<long> _sendingGroupIds = [];
        #endregion

        #region 公开属性
        public OutboxEmailAddressType Type { get; private set; } = OutboxEmailAddressType.Specific;

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

        /// <summary>
        /// SMTP 服务器地址
        /// </summary>
        public string SmtpHost => _outbox.SmtpHost;

        /// <summary>
        /// SMTP 端口
        /// </summary>
        public int SmtpPort => _outbox.SmtpPort;

        /// <summary>
        /// 开启 SSL
        /// </summary>
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
        /// 错误原因
        /// </summary>
        public string ErroredMessage { get; private set; }

        /// <summary>
        /// 是否应释放
        /// </summary>
        public bool ShouldDispose { get; private set; } = false;

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable
        {
            get => !ShouldDispose && !_isCooling && !_usingLock.IsLocked;
        }
        #endregion

        #region 构造函数
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
            _sendingGroupIds.Add(sendingGroupId);
            Type = type;

            sendingItemIds?.ForEach(x => _sendingItemIds.Add(x));

            CreateDate = DateTime.Now;
            Email = outbox.Email;
            Name = outbox.Name;
            Id = outbox.Id;

            ReplyToEmails = outbox.ReplyToEmails.SplitBySeparators().Distinct().ToList();
            _sentTotalToday = outbox.SentTotalToday;
            Weight = outbox.Weight > 0 ? outbox.Weight : 1;
        }
        #endregion

        #region 更新发件箱
        /// <summary>
        /// 使用 OutboxEmailAddress 更新既有的发件地址
        /// 非并发操作
        /// </summary>
        /// <param name="data"></param>
        public void Update(OutboxEmailAddress data)
        {
            // 更新类型
            Type |= data.Type;

            // 更新关联的项
            data._sendingItemIds?.ToList()
                .ForEach(x => _sendingItemIds.Add(x));
            data._sendingGroupIds?.ToList()
                .ForEach(x => _sendingGroupIds.Add(x));
        }
        #endregion

        #region 使用和冷却状态切换
        // 使用状态锁
        private readonly Locker _usingLock = new();

        // 标志
        private bool _isCooling = false;

        /// <summary>
        /// 锁定使用权
        /// </summary>
        /// <returns></returns>
        public bool LockUsing()
        {
            return _usingLock.Lock();
        }

        /// <summary>
        /// 释放使用权
        /// 需要在程序逻辑最后一刻才释放
        /// </summary>
        public void UnlockUsing()
        {
            _usingLock.Unlock();
        }

        /// <summary>
        /// 设置冷却
        /// 若设置失败，则返回 false
        /// </summary>
        /// <returns></returns>
        public void ChangeCoolingSate(bool cooling)
        {
            _isCooling = cooling;
        }
        #endregion

        #region 外部调用，改变内部状态

        /// <summary>
        /// 是否被禁用
        /// </summary>
        /// <returns></returns>
        public bool IsLimited()
        {
            return this._sentTotalToday >= this.MaxSendCountPerDay;
        }

        /// <summary>
        /// 是否包含指定的发件组
        /// </summary>
        /// <param name="sendingGroupId"></param>
        /// <returns></returns>
        public bool ContainsSendingGroup(long sendingGroupId)
        {
            return _sendingGroupIds.Contains(sendingGroupId);
        }

        /// <summary>
        /// 标记应该释放
        /// </summary>
        /// <param name="erroredMessage"></param>
        public void MarkShouldDispose(string erroredMessage)
        {
            ErroredMessage = erroredMessage;
            ShouldDispose = true;
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

            var userReader = await DBCacher.GetCache<UserInfoCache>(sendingContext.SqlContext, UserId.ToString());
            var userSetting = await DBCacher.GetCache<OrganizationSettingCache>(sendingContext.SqlContext, userReader.OrganizationObjectId);

            // 本身有限制时，若已经达到发送上限，则不再发送
            if (MaxSendCountPerDay > 0)
            {
                if (SentTotalToday > MaxSendCountPerDay)
                {
                    ShouldDispose = true;
                }
            }
            // 本身没限制，使用系统的限制
            else if (userSetting.MaxSendCountPerEmailDay > 0 && SentTotalToday >= userSetting.MaxSendCountPerEmailDay)
            {
                ShouldDispose = true;
            }

            // 若是发件连接失败，则移除
            if (sendingContext.SendResult.SentStatus.HasFlag(SentStatus.OutboxError)
                || sendingContext.SendResult.SentStatus.HasFlag(SentStatus.EmptySendingGroup))
            {
                ShouldDispose = true;
            }

            // 判断发件箱是否需要释放
            if (sendingContext.OutboxEmailAddress.ShouldDispose)
            {
                // 受影响的发件任务
                var sendingGroupIds = sendingContext.OutboxEmailAddress._sendingGroupIds;
                // 移除指定发件箱的发件项
                foreach (var sendingGroupId in sendingGroupIds)
                {
                    if (!sendingContext.UserSendingGroupsPool.TryGetValue(sendingGroupId, out var sendingGroup)) continue;

                    // 判断当前发件组是否还有发件箱
                    var existOtherOutbox = sendingContext.UserOutboxesPool.Values.Any(x => x.Email != sendingContext.OutboxEmailAddress.Email
                        && x._sendingGroupIds.Contains(sendingGroupId));
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
