namespace InntalerSchachfreundeTest
{
    using FluentAssertions;
    using InntalerSchachfreunde;
    using InntalerSchachfreunde.Entities;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.ObjectModel;
    using Xunit;

    public class UnitTest1
    {
        [Fact]
        public async void TestAddPlayers()
        {
            using var context = SetupDb();
            var player1 = new Player
            {
                Name = "player1",               
            };
            var player2 = new Player
            {
                Name = "player2",
            };
            context.Players.Add(player1);
            context.Players.Add(player2);
            await context.SaveChangesAsync();

            var players = context.Players.Select(p => p).Count().Should().Be(2);
            
        }

        [Fact]
        public async void TestAddGames()
        {
            using var context = SetupDb();
            
            var player1 = new Player
            {
                Id = 1,
                Name = "player1",
            };
            var player2 = new Player
            {
                Id = 2,
                Name = "player2",
            };
            context.Players.Add(player1);
            context.Players.Add(player2);
            await context.SaveChangesAsync();

            var game1 = new Game()
            {
                PlayerBlackId = player2.Id,
                PlayerWhiteId = player1.Id,
                PointsWhite = 1,
            };
            context.Games.Add(game1);
            await context.SaveChangesAsync();


            context.Players.Single(p => p.Id == 1).Should().NotBeNull();
            context.Players.Single(p => p.Id == 1).Games.Should().NotBeNull();
            context.Players.Single(p => p.Id == 1).Games.Count().Should().Be(1);
            context.Players.Single(p => p.Id == 1).Games.First().PlayerBlack.Id.Should().Be(2);
        }
        [Fact]
        public async void TestAddGamesAndTournament()
        {
            using var context = SetupDb();

            var player1 = new Player
            {
                Id = 1,
                Name = "player1",               
            };
            var player2 = new Player
            {
                Id = 2,
                Name = "player2",
            };
            var player3 = new Player
            {
                Id = 3,
                Name = "player3",
            };
            context.Players.Add(player1);
            context.Players.Add(player2);
            context.Players.Add(player3);
            await context.SaveChangesAsync();
            var tournament = new Tournament()
            {
                Id = 1,
                Name = "TestMeisterschaft",               
            };

            context.Tournaments.Add(tournament);
            await context.SaveChangesAsync();

            var tournamentPlayers1 = new PlayerTournament(){ PlayerId = 1, TournamentId = 1};
            var tournamentPlayers2 = new PlayerTournament(){ PlayerId = 2, TournamentId = 1};
            var tournamentPlayers3 = new PlayerTournament(){ PlayerId = 3, TournamentId = 1};
            context.PlayerTournament.Add(tournamentPlayers1);
            context.PlayerTournament.Add(tournamentPlayers2);
            context.PlayerTournament.Add(tournamentPlayers3);

            await context.SaveChangesAsync();

            var game1 = new Game()
            {
                PlayerWhiteId = player1.Id,
                PlayerBlackId = player2.Id,
                PointsWhite = 1,
                TournamentId = 1
            };
            var game2 = new Game()
            {
                PlayerWhiteId = player1.Id,
                PlayerBlackId = player3.Id,
                PointsWhite = 0,
                TournamentId = 1

            };
            var game3 = new Game()
            {
                PlayerWhiteId = player2.Id,
                PlayerBlackId = player3.Id,
                PointsWhite = 1,
                TournamentId = 1

            };
            var game4 = new Game()
            {
                PlayerWhiteId = player2.Id,
                PlayerBlackId = player1.Id,
                PointsWhite = 0.5,
                TournamentId = 1

            };
            var game5 = new Game()
            {
                PlayerWhiteId = player3.Id,
                PlayerBlackId = player1.Id,
                PointsWhite = 1,
                TournamentId = 1

            };
            var game6 = new Game()
            {
                PlayerWhiteId = player3.Id,
                PlayerBlackId = player2.Id,
                PointsWhite = 0,
                TournamentId = 1

            };
            context.Games.Add(game1);
            context.Games.Add(game2);
            context.Games.Add(game3);
            context.Games.Add(game4);
            context.Games.Add(game5);
            context.Games.Add(game6);
            await context.SaveChangesAsync();

            context.Tournaments.First().Games.Count.Should().Be(6);
            context.Players.Single(p => p.Id == 1).PlayerTournaments.Count().Should().Be(1);
            context.Players.Single(p => p.Id == 1).PlayerTournaments.First().Tournament.Games.Count.Should().Be(6);
        }
        public AppDbContext SetupDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

             return new AppDbContext(options);
        }
    }
}