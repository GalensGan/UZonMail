using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class Group
    {
        public Group()
        {
            // 初始化id
            id = id = Guid.NewGuid().ToString();
        }

        [BsonId]
        public string id { get; set; }

        public string userId { get; set; }

        /// <summary>
        /// 父组
        /// </summary>
        public string parentId { get; set; }

        public string name { get; set; }
        public string description { get; set; }

        public string groupType { get; set; } = "default";
    }
}
