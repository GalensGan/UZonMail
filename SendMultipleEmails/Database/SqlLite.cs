using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Database
{
    class SqlLite : IDatabase, IUser
    {
        public string ConnectionStr { get; set; }

        private SqlLite()
        {
            
        }        
    }
}
