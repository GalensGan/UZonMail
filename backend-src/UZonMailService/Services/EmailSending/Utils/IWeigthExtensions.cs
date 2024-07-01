using UZonMailService.Services.EmailSending.Base;

namespace UZonMailService.Services.EmailSending.Utils
{
    public static class IWeigthExtensions
    {
        /// <summary>
        /// 根据权重随机获取一个对象
        /// 若为空，说明权重总和为0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static IWeight? GetDataByWeight(this IEnumerable<IWeight>? datas)
        {
            var totalWeight = datas.Sum(x => x.Weight);
            var randomWeight = new Random().Next(0, totalWeight);
            var currentWeight = 0;
            foreach (var data in datas)
            {
                currentWeight += data.Weight;
                if (randomWeight < currentWeight)
                {
                    return data;
                }
            }
            return null;
        }
    }
}
