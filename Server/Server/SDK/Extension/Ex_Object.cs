using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.SDK.Extension
{
    public static class Ex_Object
    {
        /// <summary>
        /// 利用 jobject 更新对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="data"></param>
        public static void UpdateObject(this object obj, JObject data)
        {
            Type tt = obj.GetType();
            // 利用反射，将data转成相应的数据
            // https://stackoverflow.com/questions/5218395/reflection-how-to-get-a-generic-method
            var minfo = data.GetType()
                          .GetMethods()
                          .Where(m => m.Name == "ToObject")
                          .Select(m => new
                          {
                              Method = m,
                              Params = m.GetParameters(),
                              Args = m.GetGenericArguments()
                          })
                          .Where(x => x.Params.Length == 0
                                    && x.Args.Length == 1)
                         .Select(x => x.Method)
                         .First();

            minfo = minfo.MakeGenericMethod(new Type[] { tt });
            var updatingObj =  minfo.Invoke(data, new object[0]);

            // 查找待更新的keys
            var keys = data.Properties().AsQueryable().Select(p => p.Name).ToList();

            var properties = tt.GetProperties().Where(p => keys.Contains(p.Name));
            foreach (var prop in properties)
            {
                object value = prop.GetValue(updatingObj);
                // 给exist赋值
                prop.SetValue(obj, value);
            }
        }
    }
}
