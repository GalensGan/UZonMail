using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.SDK.Extension
{
    public static class Ex_Json
    {
        public static T ValueOrDefault<T>(this IEnumerable<JToken> jt, T default_)
        {
            if (jt == null) return default_;

            T value = jt.Value<T>();
            if (value == null) return default_;

            return value;
        }
    }
}
