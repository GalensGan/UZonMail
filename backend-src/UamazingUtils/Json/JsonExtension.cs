using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.Utils.Json
{
    public static class JsonExtension
    {
        /// <summary>
        /// 将 jToken 转换为指定类型的值，如果转换失败则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jt"></param>
        /// <param name="default_"></param>
        /// <returns></returns>
        public static T ToObjectOrDefault<T>(this JToken jt, T default_)
        {
            if (jt == null) return default_;

            T value = jt.ToObject<T>();
            if (value == null) return default_;

            return value;
        }

        /// <summary>
        /// 按路径从 Json 中读取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jt"></param>
        /// <param name="path"></param>
        /// <param name="default_"></param>
        /// <returns></returns>
        public static T SelectTokenOrDefault<T>( this JToken jt, string path, T default_)
        {
            if (string.IsNullOrEmpty(path)) return default_;
            if(jt == null) return default_;

            return jt.SelectToken(path).ToObjectOrDefault(default_);
        }
    }
}
