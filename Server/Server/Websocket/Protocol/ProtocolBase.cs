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
        /// <summary>
        /// 前端 websocket-promise 需要的id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 后端 Task 需要的 id
        /// </summary>
        public string taskId { get; set; }

        /// <summary>
        /// 前端事件监听名称
        /// </summary>
        public string eventName { get; set; }

        /// <summary>
        /// 进入事件后的command
        /// </summary>
        public string command { get; set; }

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
