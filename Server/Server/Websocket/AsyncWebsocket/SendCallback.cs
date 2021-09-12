using Newtonsoft.Json;
using Server.Protocol;
using Server.Websocket.AsyncWebsocket;
using StyletIoC;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Websocket.Temp
{
    class SendCallback : Dictionary<string, CallbackOption<ReceivedMessage>>
    {
        private static SendCallback _instance;
        public static SendCallback Insance
        {
            get
            {
                if (_instance == null) _instance = new SendCallback();
                return _instance;
            }
        }

        private SendCallback() { }

        public Task<ReceivedMessage> SendAsync(string sessionKey, ProtocolBase protocol)
        {
            // 获取session
            if (!SessionsCenter.Instance.TryGetValue(sessionKey, out WebSocketSession targetSession)) return null;

            // 保存Task,方便下次回调
            var callBack = new CallbackOption<ReceivedMessage>();

            Add(callBack.Id, callBack);
            // 修改 taskId
            protocol.taskId = callBack.Id;

            var task = callBack.Run(() =>
           {
                // 发送数据
                targetSession.Send(JsonConvert.SerializeObject(protocol));
           });

            return task;
        }

        /// <summary>
        /// 不阻塞发送，发送完成后，不会在此处返回结果
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="protocol"></param>
        public void Send(string sessionKey,ProtocolBase protocol)
        {
            // 获取session
            if (!SessionsCenter.Instance.TryGetValue(sessionKey, out WebSocketSession targetSession)) return;

            // 发送数据
            targetSession.Send(JsonConvert.SerializeObject(protocol));
        }
    }
}
