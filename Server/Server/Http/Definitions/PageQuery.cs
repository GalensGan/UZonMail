using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Definitions
{
    /// <summary>
    /// 分页查询实体
    /// </summary>
    internal class PageQuery
    {
        public Filter filter { get; set; }
        public Pagination pagination { get; set; }
    }

    internal class Filter
    {
        public string filter { get; set; }
    }

    internal class Pagination
    {
        public string sortBy { get; set; }
        public bool descending { get; set; }
        public int skip { get;set; }
        public int limit { get;set; }
    }
}
