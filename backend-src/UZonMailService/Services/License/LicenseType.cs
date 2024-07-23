namespace UZonMailService.Services.License
{
    /// <summary>
    /// 授权类型
    /// </summary>
    public enum LicenseType
    {
        /// <summary>
        /// 社区版本，免费
        /// </summary>
        Community = 1 << 0,

        /// <summary>
        /// 专业版
        /// </summary>
        Professional = 1 << 1,

        /// <summary>
        /// 企业版
        /// </summary>
        Enterprise = 1 << 2,
    }
}
