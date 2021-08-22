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
        public string userId { get; set; }

        public string userName { get; set; }
        public string email { get; set; }

        [BsonRef("Group")]
        public Group group { get; set; }
    }
}
