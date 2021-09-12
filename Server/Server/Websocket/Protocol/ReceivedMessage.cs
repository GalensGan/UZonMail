using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Logging;
using SuperSocket.WebSocket;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Server.Protocol
{
    public class ReceivedMessage
    {
        private string _message;

        private Dictionary<string, object> _datas = new Dictionary<string, object>();
        public ReceivedMessage(WebSocketSession session, string message)
        {
            Session = session;
            Logger = session.Logger;
            _message = message;

            JObject = JsonConvert.DeserializeObject<JObject>(message);
            Body = JsonConvert.DeserializeObject<BodyBase>(message);
            Body.Session = session;
        }

        public ILog Logger { get; set; }

        public WebSocketSession Session { get; private set; }

        /// <summary>
        /// 接收的数据，类的形式
        /// </summary>
        public BodyBase Body { get; private set; }

        /// <summary>
        /// 接收的数据，json的形式
        /// </summary>
        public JObject JObject { get; private set; }

        public T Data<T>() where T:ProtocolBase
        {
            if (_datas.TryGetValue(typeof(T).Name, out object obj))
            {
                return (T)obj;
            }
            else
            {
                T res = JsonConvert.DeserializeObject<T>(_message);
                res.Session = Session;
                _datas.Add(typeof(T).Name, res);
                return res;
            }
        }

        /// <summary>
        /// 处理完成后回复，此处是回复特定的频道
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="result"></param>
        /// <param name="status"></param>
        /// <param name="statusText"></param>
        public void Response(string eventName,object result,int status = 200,string statusText = "success")
        {
            Response response = new Response(eventName)
            {
                status = status,
                statusText = statusText,
                result = result
            };
            Response(response);
        }

        /// <summary>
        /// 处理完成后回复，此处直接原路回复
        /// </summary>
        /// <param name="result"></param>
        /// <param name="status"></param>
        /// <param name="statusText"></param>
        public void Response(object result, int status = 200, string statusText = "success")
        {
            Response response = new Response(this.Body)
            {
                status = status,
                statusText = statusText,
                result = result
            };
            Response(response);
        }

        /// <summary>
        /// 处理完成后回复
        /// </summary>
        /// <param name="response"></param>
        public void Response(Response response)
        {
            this.Session.Send(response.SerializeObject());
        }

        /// <summary>
        /// 在没有触发命令时调用
        /// </summary>
        public void ResponseDefault()
        {
            this.Response(null, 204, $"未找到执行 {Body.Command} 命令的相关逻辑");
        }

        /// <summary>
        /// 生成一个 HttpClient
        /// </summary>
        /// <param name="progressMessageHandler"></param>
        /// <returns></returns>
        public HttpClient CreateHttpClient(HttpMessageHandler messageHandler = null)
        {
            // 添加拦截器
            HttpClient httpClient = new HttpClient(messageHandler);

            AuthenticationHeaderValue authentication = new AuthenticationHeaderValue(
                               "authorization",
                               Body.token);
            httpClient.DefaultRequestHeaders.Authorization = authentication;
            return httpClient;
        }
    }
}
