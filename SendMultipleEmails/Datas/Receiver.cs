using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    public class Receiver:Person
    {
        public int GroupId { get; set; }

        [LiteDB.BsonIgnore]
        public string GroupFullName { get; set; }        
    }
}
