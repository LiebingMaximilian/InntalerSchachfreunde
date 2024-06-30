namespace InntalerSchachfreunde.Entities
{
    public class PlayerTournament
    {
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
    }
}
