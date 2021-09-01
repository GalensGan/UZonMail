using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
   public class EmailInfo:AutoObjectId
    {
        public string userName { get; set; }
        public string email { get; set; }
        public string groupId { get; set; }
    }
}
