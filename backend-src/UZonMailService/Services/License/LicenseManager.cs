using Uamazing.Utils.Web.Service;

namespace UZonMailService.Services.License
{
    /// <summary>
    /// 授权管理
    /// </summary>
    public class LicenseManager(IServiceScopeFactory ssf) : ISingletonService
    {
        /// <summary>
        /// 上一次授权更新日期
        /// </summary>
        private DateTime _lastUpdateDate;

        /// <summary>
        /// 更新间隔
        /// </summary>
        private double _updateIntervalHours = 24;

        /// <summary>
        /// 获取授权类型
        /// </summary>
        /// <returns></returns>
        public LicenseType GetLicenseType()
        {
            // 判断是否需要更新
            var timespan = DateTime.Now - _lastUpdateDate;
            if (timespan.TotalHours > _updateIntervalHours)
            {
                // 更新授权
                UpdateLicense();
            }

            return _licenseType;
        }

        private LicenseType _licenseType = LicenseType.Community;
        private void UpdateLicense()
        {
            // 从数据库读取授权文件
            _licenseType = LicenseType.Community;
        }
    }
}
