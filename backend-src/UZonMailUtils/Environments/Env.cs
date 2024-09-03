namespace Uamazing.Utils.Environments
{
    public class Env
    {
#if DEBUG
        /// <summary>
        /// 是否是调试模式
        /// </summary>
        public static bool IsDebug { get; set; } = true;
#else
        public static bool IsDebug { get; set; } = false;
#endif
    }
}
