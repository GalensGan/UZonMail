using LiteDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class HistoryGroup
    {
        [BsonId]
        public int _id { get; set; }
        public string userId { get; set; }

        public List<int> senderIds { get; set; }
        public List<int> ReceiverIds { get; set; }

        public DateTime createDate { get; set; }

        public string subject { get; set; }
        public int templateId { get; set; }
        public JArray data { get; set; }
    }
}
