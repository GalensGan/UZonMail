using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class EmailInfo : AutoObjectId
    {
        /// <summary>
        /// name 具有唯一性
        /// </summary>
        public string userName { get; set; }

        public string email { get; set; }
        public string groupId { get; set; }

        public override string GetFilterString()
        {
            return $"{base.GetFilterString()}{userName}{email}";
        }
    }
}
