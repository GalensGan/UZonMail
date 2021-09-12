using Newtonsoft.Json;

namespace Server.Protocol
{
    /// <summary>
    /// 返回时传递
    /// </summary>
    public class Response : ProtocolBase
    {
        /// <summary>
        /// 在 http 中使用，websocket 中请不要使用
        /// </summary>
        public Response()
        {
            // 默认构造
        }

        public Response(ProtocolBase protocol)
        {
            id = protocol.id;
        }

        public Response(BodyBase body)
        {
            id = body.id;
            ignoreError = body.ignoreError;
        }

        // 传入 channel 名称
        public Response(string eventName)
        {
            this.eventName = eventName;
        }

        /// <summary>
        /// 返回时忽略错误，不进行拦截
        /// </summary>
        public bool ignoreError { get; set; } = false;

        /// <summary>
        /// 状态码
        /// </summary>
        public int status { get; set; } = 200;

        /// <summary>
        /// 状态消息
        /// </summary>
        public string statusText { get; set; } = "ok";

        /// <summary>
        /// 结果
        /// </summary>
        public object result { get; set; }


        public string SerializeObject()
        {
            string result = JsonConvert.SerializeObject(this);
            return result;
        }
    }
}
