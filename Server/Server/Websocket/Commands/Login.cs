using Server.Config;
using Server.Execute;
using Server.Http.Headers;
using Server.Protocol;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Websocket.Commands
{
    class Login : IWebsocketCommand
    {
        public ILog Logger { get; set; }
        public string Name => CommandClassName.Login.ToString();

        public void ExecuteCommand(ReceivedMessage message)
        {
            // 添加session
           var sessionCenter =  Temp.SessionsCenter.Instance;

            // 不提示错误，直接返回
            // 前端可能还没登陆
            if (string.IsNullOrEmpty(message.Body.token))
            {
                message.Response(false);
                return;
            }

            // 获取token
            var jwtToken = new JwtToken(UserConfig.Instance.TokenSecret, message.Body.token);

            if (!sessionCenter.ContainsKey(jwtToken.UserId))
            {
                // 将 session 添加到缓存中
                sessionCenter.Add(jwtToken.UserId, message.Session);
            }
            else
            {
                sessionCenter[jwtToken.UserId] = message.Session;
            }

            // 发送成功
            message.Response(true);
        }
    }
}
