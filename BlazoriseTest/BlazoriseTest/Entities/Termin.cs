namespace InntalerSchachfreunde.Entities
{
    public class Termin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string? Annotation { get; set; }
        public bool? IsCanceled { get; set; }
    }
}
