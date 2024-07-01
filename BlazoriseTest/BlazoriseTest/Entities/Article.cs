namespace InntalerSchachfreunde.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Headline { get; set; }
        public string? Text { get; set; }
        public DateTime ReleaseDate { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
