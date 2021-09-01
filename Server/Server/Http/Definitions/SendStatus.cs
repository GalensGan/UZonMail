using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Definitions
{
    public enum SendStatus
    {
        Init=1,
        Sending=2,
        Resending=3,
        Pause=4,
        SendFinish=0,
    }
}
