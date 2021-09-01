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
    }
}
