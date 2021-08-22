using LiteDB;
using Server.Database.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Extensions
{
    public static class Ex_LiteRepository
    {
        public static T Upsert2<T>(this ILiteRepository repository, Expression<Func<T,bool>> filter,T data, UpdateOptions options=null)
        {
            // 查找是否存在
            T exist = repository.Query<T>().Where(filter).FirstOrDefault();
            // 如果不存在，新建
            if (exist == null)
            {
                repository.Insert(data);
                return data;
            }

            // 更新数据
            Type tt = data.GetType();
            var properties = tt.GetProperties().Where(p=> options == null || options.Validate(p.Name));
            foreach(var prop in properties)
            {
                object value = prop.GetValue(data);
                // 给exist赋值
                prop.SetValue(exist, value);              
            }

            // 更新到数据库
            repository.Upsert(exist);
            return exist;
        }
    }
}
