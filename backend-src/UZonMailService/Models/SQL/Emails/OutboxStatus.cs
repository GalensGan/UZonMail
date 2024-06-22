namespace UZonMailService.Models.SqlLite.Emails
{
    public enum OutboxStatus
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown,

        /// <summary>
        /// 有效
        /// </summary>
        Valid,

        /// <summary>
        /// 不可用
        /// </summary>
        Invalid,
    }
}
