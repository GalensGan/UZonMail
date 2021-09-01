using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class Template:AutoObjectId
    {
        public string imageUrl { get; set; }
        public string html { get; set; }
        public string name { get; set; }
        public string userId { get; set; }
        public DateTime createDate { get; set; }
    }
}
