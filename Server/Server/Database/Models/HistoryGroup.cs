using LiteDB;
using Newtonsoft.Json.Linq;
using Server.Http.Controller;
using Server.Http.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class HistoryGroup:AutoObjectId
    {
        public string userId { get; set; }

        public List<string> senderIds { get; set; }
        public List<string> receiverIds { get; set; }

        public DateTime createDate { get; set; }

        public string subject { get; set; }
        public string templateId { get; set; }
        public string templateName { get; set; }
        // json 格式的数据
        public string data { get; set; }

        public SendStatus sendStatus { get; set; }

        // 临时数据:发送成功的数量
        public int successCount { get; set; }
    }
}
