using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Modules.SendEmail
{
    class UserInstance<T>:Dictionary<string,T>
    {
        /// <summary>
        /// 如果为空，返回空串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new  T this[string key]
        {
            get
            {
                // 判断是否有数据
                TryGetValue(key, out T value);
                return value;
            }
            set
            {
                base[key] = value;
            }
        }

        /// <summary>
        /// 添加或者替换
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Upsert(string key,T value)
        {
            if (ContainsKey(key))
            {
                base[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }
    }
}
