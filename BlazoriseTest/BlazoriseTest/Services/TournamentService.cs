using InntalerSchachfreunde;
using InntalerSchachfreunde.Entities;
using Microsoft.EntityFrameworkCore;

public class TournamentService : ITournamentService
{

    private readonly AppDbContext _context;
    private readonly ILogger<TournamentService> _logger;

    public TournamentService(AppDbContext context, ILogger<TournamentService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<Tournament> GetTournamentByName(string name)
    {
        var tournament = await _context.Tournaments.SingleOrDefaultAsync(t => t.Name.Equals(name));
        return tournament;
    }
    public async Task<Tournament> GetTournamentByNameAsNoTracking(string name)
    {
        var tournament = await _context.Tournaments.AsNoTracking().SingleOrDefaultAsync(t => t.Name.Equals(name));
        tournament.TournamentPw = "";
        return tournament;
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
            row.Add(players.ElementAt(i).Name);
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
                        row.Add("-");
                    }
                    else
                    {
                        if(game.PointsWhite == 0.5)
                        {
                            row.Add("½");
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

    public async Task<(bool, string)> SaveGame(Game game, int tournamentid, string password)
    {
        try
        {
            var tournament = await _context.Tournaments.SingleAsync(t => t.Id == tournamentid);
            // this might seem a bit unsecure, but it just is not that important
            if(!tournament.TournamentPw.Equals(password))
            {
                return (false, "Passwort falsch");
            }
            var alreadyExists = await _context.Games.AnyAsync(g =>
                g.TournamentId == game.TournamentId &&
                (g.PlayerBlackId == game.PlayerBlackId &&
                g.PlayerWhiteId == game.PlayerWhiteId) ||
                (g.PlayerWhiteId == game.PlayerBlackId &&
                g.PlayerBlackId == game.PlayerWhiteId)
                );

            if (alreadyExists)
            {
                return (false, "This match already exits");
            }

            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return (true, "");
        }
        catch(Exception e)
        {
            _logger.LogError($"Saving Game Failed. {e.Message}");
            return (false, "");
        }

    }

    public async Task<(bool,Tournament?)> CreateTournament(string tournamentName, string tournamentpw)
    {
        bool alreadyExists = await _context.Tournaments.AnyAsync(t => t.Name.Equals(tournamentName));
        if(alreadyExists)
        {
            return (false, null);
        }
        var tournament = new Tournament() { Name = tournamentName, TournamentPw = tournamentpw };
        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();
        return (true, tournament);
    }

    public async Task<bool> AddPlayerToTournament(int playerId, int tournamentId)
    {
        try
        {
            _context.PlayerTournament.Add(new PlayerTournament() { PlayerId = playerId, TournamentId = tournamentId });
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

