using InntalerSchachfreunde.Entities;

public interface ITournamentService
{
    Task<Tournament> GetTournamentByName(string name);
    Task<string> GetCurrentTournamentName();
    Task<Tournament> GetTournamentByNameAsNoTracking(string name);
    Task<(bool, string)> SaveGame(Game game, int tournamentid, string password);
    Task<CrossTable> GetCrossTableOfTournament(string tournamentName);
}
 
