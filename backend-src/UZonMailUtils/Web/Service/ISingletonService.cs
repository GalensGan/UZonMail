using System;
using System.Collections.Generic;
using System.Text;

namespace UZonMail.Utils.Web.Service
{
    public interface ISingletonService : IService
    {
    }

    public interface ISingletonService<T> : ISingletonService
    {
    }
}
