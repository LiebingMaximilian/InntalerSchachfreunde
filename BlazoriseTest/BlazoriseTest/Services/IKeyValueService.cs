using InntalerSchachfreunde.Entities;
using Microsoft.EntityFrameworkCore;

namespace InntalerSchachfreunde.Services
{
    public interface IKeyValueService
    {
        public string GetValue(string key);
        public Task<bool> SetValue(string key, string value);
        public Task<List<KeyValue>> GetAllKeyValues();
    }
    class KeyValueService : IKeyValueService
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        public KeyValueService(AppDbContext context, ILogger logger)
        {   
            _context = context;
            _logger = logger;
        }

        public Task<List<KeyValue>> GetAllKeyValues()
        {
            try
            {
                return _context.KeyValues.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting all key values. Exception: {e}", e.Message);    
                throw;
            }        
        }

        public string? GetValue(string key)
        {
            return _context.KeyValues.FirstOrDefault(kv => kv.Key == key, null)?.Value;
        }

        public async Task<bool> SetValue(string key, string value)
        {
            try
            {
                if (!_context.KeyValues.AnyAsync(k => k.Key == key).Result)
                {
                    throw new ArgumentException("Key not found");
                }
                var entity = await _context.KeyValues.SingleAsync(k => k.Key == key);
                entity.Value = value;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("Error setting value for key {key}. Exception: {e}", key, e.Message);
                return false;
            }
        }
    }
}
