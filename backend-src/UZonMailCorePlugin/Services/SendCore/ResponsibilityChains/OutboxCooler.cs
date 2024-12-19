using log4net;
using UZonMail.Core.Services.SendCore.Contexts;
using UZonMail.Core.Services.SendCore.Utils;
using UZonMail.DB.Managers.Cache;
using UZonMail.DB.SQL;
using UZonMail.Managers.Cache;

namespace UZonMail.Core.Services.SendCore.ResponsibilityChains
{
    /// <summary>
    /// 发件箱冷却器
    /// </summary>
    public class OutboxCooler(SendingThreadsManager sendingThreadsManager,SqlContext sqlContext) : AbstractSendingHandler
    {
        private readonly static ILog _logger = LogManager.GetLogger(typeof(OutboxCooler));
        private readonly Cooler _emailCooler = new();

        protected override async Task HandleCore(SendingContext context)
        {
            // 判断是否可以继续使用，受限于发件总数的限制
            // 没有成功，则可继续使用
            if (!context.Status.HasFlag(ContextStatus.Success)) return;
                        
            var outbox = context.OutboxAddress;
            if (outbox == null) return;
            // 被限制后，直接返回
            if (outbox.IsLimited()) return;

            // 计算冷却时间
            var userInfo = await DBCacher.GetCache<UserInfoCache>(sqlContext, outbox.UserId.ToString());
            var orgSetting = await DBCacher.GetCache<OrganizationSettingCache>(sqlContext, userInfo.OrganizationObjectId);
            int cooldownMilliseconds = orgSetting.GetCooldownMilliseconds();
            if (cooldownMilliseconds <= 0) return;

            _logger.Debug($"发件箱 {outbox.Email} 进入冷却状态，冷却时间 {cooldownMilliseconds} 毫秒");
            outbox.ChangeCoolingSate(true);
            _emailCooler.StartCooling(cooldownMilliseconds, () =>
            {
                outbox.ChangeCoolingSate(false);
                sendingThreadsManager.StartSending(1);
                _logger.Debug($"发件箱 {outbox.Email} 退出冷却状态");
            });
        }
    }
}
