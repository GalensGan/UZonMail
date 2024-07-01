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
            if (this.Count == 0) return new FuncResult<TValue>()
            {
                Ok = false,
                Status = PoolResultStatus.EmptyError
            };

            var randomWeight = new Random().NextDouble();
            // 按权重区间配置数据
            foreach (var kv in this)
            {
                if (kv.Value.MinPercent >= randomWeight && kv.Value.MaxPercent < randomWeight)
                {
                    // 若可用，则返回
                    if (kv.Value.Enable)
                        return new FuncResult<TValue>()
                        {
                            Ok = true,
                            Data = kv.Value
                        };

                    // 若不可用，则将该次机会均分给其他人
                    var enableValues = this.Values.Where(x => x.Enable).ToList();
                    // 可用项为空
                    if (enableValues.Count == 0) return new FuncResult<TValue>()
                    {
                        Ok = false,
                        Status = PoolResultStatus.EmptyError
                    };

                    var randIndex = new Random().Next(0, enableValues.Count);
                    return new FuncResult<TValue>()
                    {
                        Ok = true,
                        Data = enableValues[randIndex]
                    };
                }
            }

            // 返回空，说明权重总和可能为 0
            return new FuncResult<TValue>()
            {
                Ok = false,
                Status = PoolResultStatus.EmptyError
            };
        }

        /// <summary>
        /// 移除空项
        /// </summary>
        public void RemoveEmptyItem()
        {
            var keys = this.Values.Where(x => x.IsEmpty).Select(x => x.Key).ToList();
            if (keys.Count == 0) return;

            foreach (var key in keys)
            {
                // 移除并释放
                if (this.TryRemove(key, out var value)) value.Dispose();
            }
            // 重新计算权重
            var totalWeight = this.Values.Sum(x => x.Weight);
            double sumPercent = 0;
            foreach (var kv in this)
            {
                var percent = kv.Value.Weight / totalWeight;
                kv.Value.MinPercent = sumPercent;
                kv.Value.MaxPercent = sumPercent + percent;
                sumPercent += percent;
            }
        }
    }
}
