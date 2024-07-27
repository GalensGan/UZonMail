using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using UZonMail.Core.Services.EmailSending.Base;

namespace UZonMail.Core.Services.EmailSending.Utils
{
    public static class IWeigthExtensions
    {
        /// <summary>
        /// 根据权重随机获取一个对象
        /// 若为空，说明权重总和为 0
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static FuncResult<IWeight> GetDataByWeight<TKey, TValue>(this IDictionary<TKey, TValue> keyValuePairs) where TValue : IWeight
        {
            // 获取可用的项
            var useableValues = keyValuePairs.Values.Where(x => x.Enable).ToList();
            if (useableValues.Count == 0) return new FuncResult<IWeight>()
            {
                Ok = false,
                Status = PoolResultStatus.EmptyError
            };

            // 计算总的权重
            var totalWeight = useableValues.Sum(x => x.Weight);
            if (totalWeight == 0)
            {
                throw new Exception("所有发件箱的权重和为 0");
            }
            var randomWeight = new Random().Next(totalWeight);
            int total = 0;
            int index = 0;
            while (total < randomWeight)
            {
                total += useableValues[total].Weight;
                index++;
            }

            return new FuncResult<IWeight>()
            {
                Ok = true,
                Data = useableValues[index]
            };
        }
    }
}
