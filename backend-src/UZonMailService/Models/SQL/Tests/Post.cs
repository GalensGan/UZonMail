namespace UZonMailService.Models.SQL.Tests
{
    public class Post
    {
        public int Id { get; set; }
        public List<Tag> Tags { get; } = new List<Tag>();

    }
}
