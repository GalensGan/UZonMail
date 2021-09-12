using Server.Execute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Server.Protocol
{
    /// <summary>
    /// get 时传递
    /// </summary>
   public class BodyBase:ProtocolBase
    {
        /// <summary>
        /// 实现 ICommand 类的键值
        /// </summary>
        public string name = CommandClassName.None.ToString();

        /// <summary>
        /// 命令
        /// </summary>
        public string command;

        public CommandTable Command
        {
            get
            {
                if (Enum.TryParse(command, out CommandTable result))
                {
                    return result;
                }
                else return CommandTable.None;
            }
        }

        /// <summary>
        /// token 值
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 忽略错误，在返回时，不进行拦截
        /// </summary>
        public bool ignoreError { get; set; } = false;


        public AuthenticationHeaderValue GetToken()
        {
            AuthenticationHeaderValue authentication = new AuthenticationHeaderValue("authorization",token);
            return authentication;
        }
    }
}
