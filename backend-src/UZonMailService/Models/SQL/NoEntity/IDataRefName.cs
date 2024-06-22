namespace UZonMailService.Models.SQL.NoEntity
{
    /// <summary>
    /// 仅有 Name 的数据引用
    /// </summary>
    public interface IDataRefName
    {
        int Id { get; set; }
        string Name { get; set; }
    }
}
