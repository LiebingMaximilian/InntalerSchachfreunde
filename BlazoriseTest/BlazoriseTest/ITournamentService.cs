using InntalerSchachfreunde;
using InntalerSchachfreunde.Entities;
using Microsoft.EntityFrameworkCore;

public interface ITournamentService
{
    Task<Tournament> GetTournamentByName(string name);
    Task<string> GetCurrentTournamentName();
}

public class TournamentService : ITournamentService
{

    AppDbContext? _context { get; set; }

    public TournamentService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Tournament> GetTournamentByName(string name)
    {
        return await _context.Tournaments.SingleAsync(t => t.Name.Equals(name));
    }

    public async Task<string> GetCurrentTournamentName()
    {
        return _context.KeyValues.SingleAsync(kv => kv.Key.Equals("CurrentTournament")).Result?.Value ?? "";
    }
}
