namespace InntalerSchachfreunde.Entities
{
    public class Tournament
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TournamentPw { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<PlayerTournament> PlayerTournaments { get; set; }
    }
}
