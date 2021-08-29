using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
   public class EmailInfo
    {
        [BsonId]
        public int _id { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public int groupId { get; set; }
    }
}
