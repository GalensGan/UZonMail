using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    public class Template
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 生成例子
        /// </summary>
        /// <param name="key"></param>
        public void GenerateSample(TemplateSample key)
        {
            switch (key)
            {
                case TemplateSample.SalaryDetail:
                    // 生成工资明细表
                    Content = "SalaryDetail";
                    Name = "SalaryDetail";
                    break;
                case TemplateSample.Letter:
                    Content = "Letter";
                    Name = "Letter";
                    break;
                default:
                    break;
            }


        }

        public override bool Equals(object obj)
        {
            if (obj is Template p)
            {
                return p.Name.Equals(this.Name);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public enum TemplateSample
    {
        SalaryDetail,
        Letter,
    }
}
