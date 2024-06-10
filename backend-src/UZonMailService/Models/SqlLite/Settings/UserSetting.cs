using UZonMailService.Models.SqlLite.Base;

namespace UZonMailService.Models.SqlLite.Settings
{
    /// <summary>
    /// 用户设置
    /// </summary>
    public class UserSetting : SqlId
    {
        /// <summary>
        /// 用户 id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 每日每个发件箱最大发送次数
        /// 为 0 时表示不限制
        /// </summary>
        public int MaxSendCountPerEmailDay { get; set; } = 0;

        /// <summary>
        /// 最小发件箱冷却时间
        /// </summary>
        public int MinOutboxCooldownSecond { get; set; } = 5;

        /// <summary>
        /// 最大发件箱冷却时间
        /// </summary>
        public int MaxOutboxCooldownSecond { get; set; } = 10;

        /// <summary>
        /// 最大批量发件数
        /// </summary>
        public int MaxSendingBatchSize { get; set; } = 20;

        /// <summary>
        /// 获取冷却时间
        /// 随机
        /// </summary>
        /// <returns></returns>
        public int GetCooldownMilliseconds()
        {
            if(MinOutboxCooldownSecond == MaxOutboxCooldownSecond)
            {
                return MinOutboxCooldownSecond * 1000;
            }

            // 随机从 min 到 max 取值
            return new Random().Next(MinOutboxCooldownSecond, MaxOutboxCooldownSecond) * 1000;
        }
    }
}
