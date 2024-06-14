namespace UZonMailService.Models.SqlLite.NoEntity
{
    /// <summary>
    /// 数据的所有引用
    /// </summary>
    public interface IDataRef : IDataRefName
    {
        string Description { get; set; }
    }
}
