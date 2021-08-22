using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Response
{
    public class HttpResponse
    {
        public int code { get; set; } = 20000;
        public string message { get; set; }
        public object data { get; set; }
    }
}
