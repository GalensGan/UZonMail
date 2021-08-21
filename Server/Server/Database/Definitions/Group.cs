using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Definitions
{
    public class Group
    {
        [BsonRef("User")]
        public User user { get; set; }

        /// <summary>
        /// 父组
        /// </summary>
        [BsonRef("Group")]
        public Group parent { get; set; }

        public string name { get; set; }
        public string description { get; set; }
    }
}
