using InntalerSchachfreunde.Entities;

public interface ITournamentService
{
    Task<Tournament> GetTournamentByName(string name);
    Task<string> GetCurrentTournamentName();
    Task<Tournament> GetTournamentByNameAsNoTracking(string name);
    Task<(bool, string)> SaveGame(Game game, int tournamentid, string password);
    Task<CrossTable> GetCrossTableOfTournament(string tournamentName);
    Task<(bool, Tournament?)> CreateTournament(string tournamentName, string tournamentpw);
    Task<bool> AddPlayerToTournament(int playerId, int tournamentId);
    Task<List<Game>> GetScheduledGames(string tournamentName);
    Task GenerateRoundRobinSchedule(int tournamentId);

}
 
