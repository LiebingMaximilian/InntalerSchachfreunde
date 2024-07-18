using InntalerSchachfreunde.Entities;
using Microsoft.EntityFrameworkCore;
namespace InntalerSchachfreunde.Services

{
    public interface ITerminService
    {
        public List<Termin> GetTermine();
        public Task<Termin> CreateTermin(Termin id);
        public Task DeleteTermin(int id);
    }
    public class TerminService : ITerminService
    {
        private readonly AppDbContext _context;
        public TerminService(AppDbContext context)
        {
            _context = context;
        }
        public List<Termin> GetTermine()
        {
            return _context.Termins.Where(t => t.DateTime > DateTime.Now)
                .OrderBy(t => t.DateTime)
                .AsParallel()
                .ToList();
        }
        public async Task<Termin> CreateTermin(Termin termin)
        {
            _context.Termins.Add(termin);
            await _context.SaveChangesAsync();
            return termin;
        }

        public Task DeleteTermin(int id)
        {
           _context.Termins.Remove(_context.Termins.Find(id));
            return _context.SaveChangesAsync();
        }
    }
}
