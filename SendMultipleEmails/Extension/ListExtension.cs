using NPOI.HSSF.Record;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Extension
{
    public static class ListExtension
    {     
        /// <summary>
        /// 筛选重复元素
        /// </summary>
        /// <typeparam name="TSource">要去重的对象类，委托输入类型</typeparam>
        /// <typeparam name="TKey">自定义去重的字段类型,委托返回类型</typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                TKey elementValue = keySelector(element);
                if (seenKeys.Add(elementValue))
                {
                    yield return element;
                }
            }
        }
    }
}
