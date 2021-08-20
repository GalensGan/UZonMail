using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Protocol
{
   public class ProtocolBase
    {
        public string id { get; set; }
        public string channelName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public WebSocketSession Session { get; set; }

        // 数据验证
        public virtual bool Validate() { return true; }

        public void SendMessage(string message)
        {
            if (Session == null || !Session.Connected) return;
            Response response = new Response(this)
            {
                status = 201,
                statusText = message
            };
            Session.Send(response.SerializeObject());
        }
    }
}
