namespace InntalerSchachfreunde.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<PlayerTournament> PlayerTournaments { get; set; }
    }
}
