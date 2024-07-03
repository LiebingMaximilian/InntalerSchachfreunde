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

    public async Task<PlayerGamesDto> GetPlayersAndGamesOfTournament(Tournament tournament)
    { 
        var players = tournament.PlayerTournaments.Select(t => t.Player).ToList();

        var games = tournament.Games?.ToList();
        return new PlayerGamesDto()
        {
            Games = games,
            Players = players
        };
    }
    public async Task<CrossTable> GetCrossTableOfTournament(string tournamentName)
    {
        var tournament = await GetTournamentByName(tournamentName);
        var playersTournaments = await GetPlayersAndGamesOfTournament(tournament);

        var players = playersTournaments.Players.OrderBy(p => p.Name);

        var games = playersTournaments.Games;

        var crossTable = new CrossTable()
        {
            Header = players.Select(p => p.Name).ToList(),
        };
        var rowlist = new List<List<string>>();

        for(int i = 0; i< players.Count(); i++)
        {
            var row = new List<string>();
            for(int j = 0; j< players.Count(); j++)
            {
                if(i == j)
                {
                    row.Add("X");
                }
                else
                {
                    if(games is null || games.Count == 0)
                    {                      
                        row.Add("");
                        continue;
                    }
                    var game = games.FirstOrDefault(g => (g.PlayerWhiteId == players.ElementAt(i).Id && g.PlayerBlackId == players.ElementAt(j).Id) || (g.PlayerWhiteId == players.ElementAt(j).Id && g.PlayerBlackId == players.ElementAt(i).Id));
                    if(game is null)
                    {
                        row.Add("");
                    }
                    else
                    {
                        if(game.PointsWhite == 0.5)
                        {
                            row.Add("�");
                        }
                        else
                        {
                            if(game.PlayerWhiteId == players.ElementAt(i).Id)
                            {
                                row.Add(game.PointsWhite == 1 ? "1" : "0");
                            }
                            else
                            {
                                row.Add(game.PointsWhite == 0 ? "1" : "0");
                            }
                        }
                    }
                }
            }
            rowlist.Add(row);
        }
        crossTable.Rows = rowlist;
        return crossTable;
    }
    
}
public class PlayerGamesDto
{     
    public List<Player> Players { get; set; }
    public List<Game> Games { get; set; }
}
public class CrossTable
{
    public List<string> Header { get; set; }
    public List<List<string>> Rows { get; set; }

}

