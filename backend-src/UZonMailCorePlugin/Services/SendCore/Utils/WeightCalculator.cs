namespace UZonMail.Core.Services.SendCore.Utils
{
    /// <summary>
    /// 权重计算器
    /// </summary>
    public class WeightCalculator<TKey>
    {
        public int TotalWeight { get; private set; }
        private readonly Dictionary<TKey,int> _weights = [];

        public void Add(TKey key, int weight)
        {
           // 若存在，则更新权重
        }
    }
}
