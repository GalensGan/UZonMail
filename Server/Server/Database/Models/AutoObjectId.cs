using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public abstract class AutoObjectId
    {
        [BsonId]
        public string _id { get; set; }
        public AutoObjectId()
        {
            _id = ObjectId.NewObjectId().ToString();
        }

        /// <summary>
        /// 获取过滤字符串
        /// </summary>
        /// <returns></returns>
        public virtual string GetFilterString()
        {
            return _id;
        }

        /// <summary>
        /// 通过字段名称获取值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public object GetValue(string fieldName)
        {
            var propertyInfo = GetType().GetProperty(fieldName);
            if (propertyInfo == null) return string.Empty;

            return propertyInfo.GetValue(this, null);
        }
    }
}
