namespace UZonMailService.Models.SQL.Tests
{
    public class Tag
    {
        public int Id { get; set; }
        public List<Post> Posts { get; } = new List<Post>();
    }
}
