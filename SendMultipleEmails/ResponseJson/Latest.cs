using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.ResponseJson
{
    public class Latest
    {
        public string url;
        public string tag_name;
        public string assets_url;
        public Asset[] assets;
    }
}
