using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using InntalerSchachfreunde;
using InntalerSchachfreunde.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;

[Collection("Test")]
public class TournamentServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        // Use a unique database name for each context instance to avoid shared state between tests
        var dbName = $"InMemoryTournamentDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        var dbContext = new AppDbContext(options);
        return dbContext;
    }


    [Fact]
    public async Task GetCrossTableOfTournament_WithNoGames_ShouldReturnCorrectStructure()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var logger = Mock.Of<ILogger<TournamentService>>();

        var tournament = new Tournament { Name = "TestTournament",
        PlayerTournaments = new List<PlayerTournament>()};
        var player1 = new Player { Name = "Player1" };
        var player2 = new Player { Name = "Player2" };
        tournament.PlayerTournaments.Add(new PlayerTournament { Player = player1 });
        tournament.PlayerTournaments.Add(new PlayerTournament { Player = player2 });
        context.Tournaments.Add(tournament);
        context.SaveChanges();

        var service = new TournamentService(context, logger);

        // Act
        var crossTable = await service.GetCrossTableOfTournament("TestTournament");

        // Assert
        crossTable.Header.Should().Contain(new[] { "Player1", "Player2" });
        crossTable.Rows.Should().HaveCount(2);
        crossTable.Rows.All(row => row.Contains("X") && row.Count == 2).Should().BeTrue();
    }

    [Fact]
    public async Task GetCrossTableOfTournament_WithCompletedGames_ShouldReflectGameOutcomes()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var logger = Mock.Of<ILogger<TournamentService>>();

        var tournament = new Tournament {
            Id = 1,
            Name = "TestTournament",
            PlayerTournaments = new List<PlayerTournament>() 
        };
        var player1 = new Player { Name = "Player1", Id = 1 };
        var player2 = new Player { Name = "Player2", Id = 2 };
        var player3 = new Player { Name = "Player3", Id = 3 };
        tournament.PlayerTournaments.Add(new PlayerTournament { Player = player1 });
        tournament.PlayerTournaments.Add(new PlayerTournament { Player = player2 });
        tournament.PlayerTournaments.Add(new PlayerTournament { Player = player3 });
        tournament.Games = new List<Game>();
        tournament.Games.Add(new Game { PlayerWhiteId = 1, PlayerBlackId = 2, PointsWhite = 1, Tournament = tournament, TournamentId = 1});
        tournament.Games.Add(new Game { PlayerWhiteId = 3, PlayerBlackId = 1, PointsWhite = 0, Tournament = tournament, TournamentId = 1});
        tournament.Games.Add(new Game { PlayerWhiteId = 2, PlayerBlackId = 3, PointsWhite = 0.5, Tournament = tournament, TournamentId = 1});
        context.Tournaments.Add(tournament);
        context.SaveChanges();

        var service = new TournamentService(context, logger);

        // Act
        var crossTable = await service.GetCrossTableOfTournament("TestTournament");

        // Assert
        crossTable.Header.Should().Contain(new[] { "Player1", "Player2" });
        crossTable.Rows[0][1].Should().Be("1"); // Player1 wins against Player2
        crossTable.Rows[1][0].Should().Be("0"); // Player2 loses to Player1
        crossTable.Rows[2][0].Should().Be("0"); // Player3 loses against Player1
        crossTable.Rows[2][1].Should().Be("½");
    }
    [Fact]
    public async Task SaveGame_GameDoesNotExist_ShouldSaveSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var logger = Mock.Of<ILogger<TournamentService>>();
        var service = new TournamentService(context, logger);
        var tournament = new Tournament { Name = "TestTournament" };
        context.Tournaments.Add(tournament);
        await context.SaveChangesAsync();

        var game = new Game
        {
            Tournament = tournament,
            PlayerWhiteId = 1,
            PlayerBlackId = 2,
            PointsWhite = 1
        };

        // Act
        var (success, message) = await service.SaveGame(game);

        // Assert
        success.Should().BeTrue();
        message.Should().BeEmpty();
        context.Games.Should().ContainSingle();
    }

    [Fact]
    public async Task SaveGame_GameExists_ShouldNotSave()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var logger = Mock.Of<ILogger<TournamentService>>();
        var service = new TournamentService(context, logger);
        var tournament = new Tournament { Name = "TestTournament" };
        context.Tournaments.Add(tournament);
        var existingGame = new Game
        {
            Tournament = tournament,
            PlayerWhiteId = 1,
            PlayerBlackId = 2,
            PointsWhite = 1
        };
        context.Games.Add(existingGame);
        await context.SaveChangesAsync();

        var newGame = new Game
        {
            Tournament = tournament,
            PlayerWhiteId = 1,
            PlayerBlackId = 2,
            PointsWhite = 0.5 // Different result, but same players and tournament
        };

        // Act
        var (success, message) = await service.SaveGame(newGame);

        // Assert
        success.Should().BeFalse();
        message.Should().Be("This match already exits");
        context.Games.Should().ContainSingle(); // No new game should be added
    }
}
