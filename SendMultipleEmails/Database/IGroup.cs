using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Database
{
    interface IGroup
    {
        IEnumerable<Group> GetAllGroups();

        int InsertGroup(Group group);
    }
}
