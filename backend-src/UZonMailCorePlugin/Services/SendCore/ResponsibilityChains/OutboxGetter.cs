using UZonMail.Core.Services.SendCore.Contexts;

namespace UZonMail.Core.Services.SendCore.ResponsibilityChains
{
    public class OutboxGetter(OutboxPools outboxesManager) : AbstractSendingHandler
    {
        public override async Task HandleCore(SendingContext context)
        {
            // 获取发件箱

            // 保存到 context 中
        }
    }
}
