
namespace UZonMailService.Services.EmailSending.Base
{
    /// <summary>
    /// 权重接口
    /// </summary>
    public interface IWeight
    {
        /// <summary>
        /// 实际权重
        /// </summary>
        public int Weight { get; }

        /// <summary>
        /// 最小权重
        /// 这个是系统自动计算后赋予的，不需要手动设置
        /// </summary>
        public double MinPercent { get; set; }

        /// <summary>
        /// 最大权重
        /// 这个是系统自动计算后赋予的，不需要手动设置
        /// </summary>
        public double MaxPercent { get; set; }
    }
}
