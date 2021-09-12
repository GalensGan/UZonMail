using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Websocket.Temp
{
    /// <summary>
    /// 保存用户的 session 方便在 http 中调用
    /// </summary>
    public class SessionsCenter : Dictionary<string, WebSocketSession>
    {
        public static SessionsCenter _instance;
        public static SessionsCenter Instance
        {
            get
            {
                if (_instance == null) _instance = new SessionsCenter();
                return _instance;
            }
        }

        private SessionsCenter() { }
    }
}
