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
        if(tournament is null)
        {
            return null;
        }
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
                    if(game is null || game.PointsWhite is null)
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

    public async Task<List<Game>> GetScheduledGames(string tournamentName)
    {
        return await _context.Games.Where(g => g.Tournament.Name.Equals(tournamentName) && g.PointsWhite == null).Include(g => g.PlayerWhite).Include(g => g.PlayerBlack).ToListAsync();
    }

    public async Task GenerateRoundRobinSchedule(int tournamentId)
    {
        var players = _context.PlayerTournament.Where(pt => pt.TournamentId == tournamentId).Select(pt => pt.Player).ToList();
        var games = new List<Game>();
        if (players.Count % 2 != 0)
        {
            players.Add(new Player() {Id = 0, Name = "[Spielfrei]" }); // Add a dummy player if odd number of players
        }

        int numRounds = players.Count - 1;
        int numMatchesPerRound = players.Count / 2;

        for (int round = 0; round < numRounds; round++)
        {
            for (int match = 0; match < numMatchesPerRound; match++)
            {
                Player white = players[match];
                Player black = players[players.Count - 1 - match];

                if (white.Name != "[Spielfrei]" && black.Name != "[Spielfrei]")
                {
                    if (match % 2 == 1 || (round % 2 == 1 && match == 0))
                    {
                        games.Add(new Game { PlayerWhite = black, PlayerBlack = white, PlayerWhiteId = black.Id, PlayerBlackId = white.Id , TournamentId=tournamentId});
                    }
                    else
                    {
                        games.Add(new Game { PlayerWhite = white, PlayerBlack = black, PlayerWhiteId = white.Id, PlayerBlackId = black.Id, TournamentId = tournamentId });
                    }
                }
            }

            players.Insert(1, players[players.Count - 1]);
            players.RemoveAt(players.Count - 1);
        }

        games = BalanceColors(games, players);
        games = AssignDates(DateOnly.FromDateTime(DateTime.Now), games, players);
        foreach(var game in games)
        {
            _context.Games.Add(game);
        }
        await _context.SaveChangesAsync();
    }

    private List<Game> BalanceColors(List<Game> games, List<Player> players)
    {
        Dictionary<int, int> colorBalance = new Dictionary<int, int>();

        foreach (var player in players)
        {
            colorBalance[player.Id] = 0;
        }

        foreach (var game in games)
        {
            colorBalance[game.PlayerWhiteId]++;
            colorBalance[game.PlayerBlackId]--;
        }

        for (int i = 0; i < games.Count; i++)
        {
            var game = games[i];
            if (colorBalance[game.PlayerWhiteId] > colorBalance[game.PlayerBlackId])
            {
                games[i] = new Game
                {
                    PlayerWhite = game.PlayerBlack,
                    PlayerBlack = game.PlayerWhite,
                    PlayerWhiteId = game.PlayerBlackId,
                    PlayerBlackId = game.PlayerWhiteId,
                    TournamentId = game.TournamentId
                };
                colorBalance[game.PlayerWhiteId] -= 2;
                colorBalance[game.PlayerBlackId] += 2;
            }
        }
        return games;
    }
    public List<Game> AssignDates(DateOnly startDate, List<Game> games, List<Player> players)
    {
        Dictionary<int, DateOnly> playerLastGameDate = new Dictionary<int, DateOnly>();
        foreach (var player in players)
        {
            playerLastGameDate[player.Id] = startDate.AddDays(-14); // Initialize to day before start
        }

        foreach (var game in games)
        {
            DateOnly gameDate = playerLastGameDate[game.PlayerWhiteId].AddDays(14);
            if (gameDate <= playerLastGameDate[game.PlayerBlackId])
            {
                gameDate = playerLastGameDate[game.PlayerBlackId].AddDays(14);
            }

            game.Date = gameDate;
            playerLastGameDate[game.PlayerWhiteId] = gameDate;
            playerLastGameDate[game.PlayerBlackId] = gameDate;
        }
        return games;
    }

}

