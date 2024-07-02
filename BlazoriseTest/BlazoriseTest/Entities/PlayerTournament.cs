namespace InntalerSchachfreunde.Entities
{
    public class PlayerTournament
    {
        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }

        public int TournamentId { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}
