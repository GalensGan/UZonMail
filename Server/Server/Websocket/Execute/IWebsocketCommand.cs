using SuperSocket.SocketBase.Logging;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Execute
{
   internal interface IWebsocketCommand
    {
        ILog Logger { get; set; }

        string Name { get; set; }       

        void ExecuteCommand(ReceivedMessage message);
    }
}
