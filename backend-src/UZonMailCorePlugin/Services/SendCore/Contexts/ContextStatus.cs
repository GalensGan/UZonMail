namespace UZonMail.Core.Services.SendCore.Contexts
{
    [Flags]
    public enum ContextStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1 << 0,

        /// <summary>
        /// 失败
        /// </summary>
        Fail = 1 << 1,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 1 << 2,

        /// <summary>
        /// 是否应该退出线程
        /// </summary>
        ShouldExitThread = 1 << 3,

        /// <summary>
        /// 跳过下一步
        /// </summary>
        SkipNext = 1 << 4,
    }
}
