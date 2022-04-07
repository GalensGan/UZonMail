using Server.Database.Models;
using Server.Http.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Database.Extensions
{
    internal static class Ex_Query
    {
        /// <summary>
        /// 获取分页数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination"></param>
        /// <param name="orderFunc"></param>
        /// <returns></returns>
        public static int GetPageDatasCount<T>(this IEnumerable<T> source, Filter filter) where T : AutoObjectId
        {
            var regex = new Regex(filter.filter);

            // 进行筛选
            var results = source.Where(h => regex.IsMatch(h.GetFilterString()));

            return results.Count();
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination"></param>
        /// <param name="orderFunc"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetPageDatas<T>(this IEnumerable<T> source, Filter filter, Pagination pagination) where T : AutoObjectId
        {
            var regex = new Regex(filter.filter);

            // 进行筛选
            var results = source.Where(h => regex.IsMatch(h.GetFilterString()));

            if (pagination.descending)
            {
                results = results.OrderByDescending(item=>item.GetValue(pagination.sortBy));
            }
            else
            {
                results = results.OrderBy(item => item.GetValue(pagination.sortBy));
            }

            return results.Skip(pagination.skip).Take(pagination.limit);
        }
    }
}
