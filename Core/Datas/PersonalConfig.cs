using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    /// <summary>
    /// 个人配置的一些记录
    /// </summary>
    public class PersonalConfig
    {
        public double SendInterval { get; set; } = 5;

        public double SendIntervalRandom { get; set; } = 1.5;

        /// <summary>
        /// 上一次使用的模板记录
        /// </summary>
        public string LastTemplatePath { get; set; }
    }
}
