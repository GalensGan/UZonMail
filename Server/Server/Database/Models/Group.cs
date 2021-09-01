using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class Group:AutoObjectId
    {
        public string userId { get; set; }

        /// <summary>
        /// 父组
        /// </summary>
        public int parentId { get; set; }

        public string name { get; set; }
        public string description { get; set; }

        public string groupType { get; set; } = "default";
    }
}
