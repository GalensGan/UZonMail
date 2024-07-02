using Org.BouncyCastle.Utilities;
using System.Collections.Concurrent;
using UZonMailService.Services.EmailSending.Utils;

namespace UZonMailService.Services.EmailSending.Base
{
    /// <summary>
    /// 并发包管理器
    /// key 相当于每个用户
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryManager<TValue> : ConcurrentDictionary<string, TValue> where TValue : IDictionaryItem
    {
        /// <summary>
        /// 根据 BagData 中的 Weight 来按照权重随机获取一个对象
        /// 返回 null 时示没有数据
        /// </summary>
        /// <returns></returns>
        public FuncResult<TValue> GetDataByWeight()
        {
            // 获取可用的项
            var useableValues = this.Values.Where(x => x.Enable).ToList();
            if (useableValues.Count == 0) return new FuncResult<TValue>()
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

            return new FuncResult<TValue>()
            {
                Ok = true,
                Data = useableValues[index]
            };
        }
    }
}
