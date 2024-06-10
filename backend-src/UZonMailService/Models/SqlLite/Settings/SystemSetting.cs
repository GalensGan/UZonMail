using UZonMailService.Models.SqlLite.Base;

namespace UZonMailService.Models.SqlLite.Settings
{
    public class SystemSetting : SqlId
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = "default";

        /// <summary>
        /// 是否已经初始化 Quartz
        /// </summary>
        public bool InitializedQuartz { get; set; }
    }
}
