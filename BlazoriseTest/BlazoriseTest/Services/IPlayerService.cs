namespace InntalerSchachfreunde.Services
{
    using InntalerSchachfreunde.Entities;
    using Microsoft.EntityFrameworkCore;

    public interface IPlayerService
    {
        Task<bool> AddPlayerToDb(string name);
    }
    public class PlayerService : IPlayerService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(AppDbContext context, ILogger<PlayerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddPlayerToDb(string name)
        {
            bool alreadyExists = await _context.Players.AnyAsync(p => p.Name.Equals(name));
            if(alreadyExists)
            {
                return false;
            }
            _context.Players.Add(new Player() { Name = name});
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
