using UZonMailService.Models.SQL.Base;

namespace UZonMailService.Models.SQL.Settings
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
