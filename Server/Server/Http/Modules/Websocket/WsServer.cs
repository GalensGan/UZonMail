using EmbedIO.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Modules.Websocket
{
    /// <summary>
    /// 未使用本模块，使用 Server.Websocket 模块
    /// </summary>
    public class WsServer : WebSocketModule
    {
        public WsServer(string urlPath) : base(urlPath,true) { }

        /// <inheritdoc />
        protected override Task OnMessageReceivedAsync(
            IWebSocketContext context,
            byte[] rxBuffer,
            IWebSocketReceiveResult rxResult)
            => SendToOthersAsync(context, Encoding.GetString(rxBuffer));

        /// <inheritdoc />
        protected override Task OnClientConnectedAsync(IWebSocketContext context)
            => Task.WhenAll(
                SendAsync(context, "Welcome to the chat room!"),
                SendToOthersAsync(context, "Someone joined the chat room."));

        /// <inheritdoc />
        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
            => SendToOthersAsync(context, "Someone left the chat room.");

        private Task SendToOthersAsync(IWebSocketContext context, string payload)
            => BroadcastAsync(payload, c => c != context);
    }
}
