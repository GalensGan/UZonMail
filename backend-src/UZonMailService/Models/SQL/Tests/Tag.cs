namespace UZonMailService.Models.SQL.Tests
{
    public class Tag
    {
        public long Id { get; set; }
        public List<Post> Posts { get; } = new List<Post>();
    }
}
