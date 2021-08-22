using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Definitions
{
    public class UpdateOptions : List<string>
    {
        public UpdateOptions() { }

        public UpdateOptions(bool isExclude)
        {
            IsExclude = isExclude;
        }

        public UpdateOptions(IList<string> keys,bool isExclude=false):base(keys)
        {
            IsExclude = isExclude;
        }

        /// <summary>
        /// 是否不包含在集合中
        /// true：不在集合中的字段才更新
        /// </summary>
        public bool IsExclude { get; set; }

        public bool Validate(string value)
        {
            if (IsExclude) return !Contains(value);

            return Contains(value);
        }
    }
}
