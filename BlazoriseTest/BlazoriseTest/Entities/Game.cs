using Microsoft.Data.SqlClient.DataClassification;

namespace InntalerSchachfreunde.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public int PlayerWhiteId { get; set; }
        public virtual Player PlayerWhite { get; set; }
        public int PlayerBlackId { get; set; }
        public virtual Player PlayerBlack { get; set; }
        public double? PointsWhite { get; set; }
        public double? PointsBlack { get { if (PointsWhite is null) return null; else return 1 - PointsWhite; } }
        public int TournamentId { get; set; }
        public virtual Tournament? Tournament { get; set; }
        public DateOnly? Date { get; set; }
    }
}
