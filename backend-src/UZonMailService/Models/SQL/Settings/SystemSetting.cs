using UZonMailService.Models.SQL.Base;

namespace UZonMailService.Models.SQL.Settings
{
    /// <summary>
    /// 系统级设置全系统只此一份
    /// 每一条记录是一个设置项
    /// </summary>
    public class SystemSetting : SqlId
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 字符串值
        /// </summary>
        public string? StringValue { get; set; }

        /// <summary>
        /// bool 值
        /// </summary>
        public bool BoolValue { get; set; }

        /// <summary>
        /// int 类型值
        /// </summary>
        public int IntValue { get; set; }
    }
}
