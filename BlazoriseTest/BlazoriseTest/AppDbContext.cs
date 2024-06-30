using InntalerSchachfreunde.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace InntalerSchachfreunde
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<KeyValue>  KeyValues { get; set; }
        public DbSet<Termin> Termins { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<PlayerTournament> PlayerTournament { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerTournament>()
                .HasKey(pt => new { pt.PlayerId, pt.TournamentId });

            modelBuilder.Entity<PlayerTournament>()
                .HasOne(pt => pt.Player)
                .WithMany(p => p.PlayerTournaments)
                .HasForeignKey(pt => pt.PlayerId);

            modelBuilder.Entity<PlayerTournament>()
                .HasOne(pt => pt.Tournament)
                .WithMany(t => t.PlayerTournaments)
                .HasForeignKey(pt => pt.TournamentId);
            //link game to tournament
            modelBuilder.Entity<Game>()
                .HasOne(p => p.Tournament)
                .WithMany(b => b.Games)
                .HasForeignKey(p => p.TournamentId);
            //link players to tournament
            modelBuilder.Entity<Article>()
                .HasMany(p => p.Images)
                .WithOne(p => p.Article)
                .HasForeignKey(p => p.ArticleId);

            //link game to players
            modelBuilder.Entity<Game>()
                .HasOne(g => g.PlayerWhite)
                .WithMany(p => p.Games)
                .HasForeignKey(g => g.PlayerWhiteId);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.PlayerBlack)
                .WithMany()
                .HasForeignKey(g => g.PlayerBlackId);

            //link images to articles
            modelBuilder.Entity<Article>()
                .HasMany(p => p.Images)
                .WithOne(p => p.Article)
                .HasForeignKey(p => p.ArticleId);


        }

    }
}
