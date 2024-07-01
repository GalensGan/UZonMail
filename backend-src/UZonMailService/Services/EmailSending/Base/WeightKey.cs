namespace UZonMailService.Services.EmailSending.Base
{
    /// <summary>
    /// 权重键值
    /// </summary>
    public class WeightKey
    {
        public string Key { get; set; }

        public int MinWeight { get; set; }

        public int MaxWeight { get; set; }
    }
}
