using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Database
{
    interface ISenderDb
    {
        bool InsertSender(Sender obj);
        bool InsertSenders(IEnumerable<Sender> objs);

        bool DeleteSender(int id);

        Sender FindOneSenderByName(string name);

        Sender FindOneSenderByEmail(string email);

        IEnumerable<Sender> FindAllSenders();

        bool UpdateSender(Sender obj);

        int UpsertSenders(IEnumerable<Sender> objs);
    }
}
