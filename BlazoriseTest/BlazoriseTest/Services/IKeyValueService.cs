using InntalerSchachfreunde.Entities;
using Microsoft.EntityFrameworkCore;

namespace InntalerSchachfreunde.Services
{
    public interface IKeyValueService
    {
        public Task<string> GetValue(string key);
        public Task<bool> SetValue(string key, string value);
        public Task<List<KeyValue>> GetAllKeyValues();
    }
    class KeyValueService : IKeyValueService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<KeyValueService> _logger;
        public KeyValueService(AppDbContext context, ILogger<KeyValueService> logger)
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

        public async Task<string?> GetValue(string key)
        {
            try 
            { 
                var kv =  await _context.KeyValues.FindAsync(key);
                if (kv is not null)
                {
                    return kv.Value;
                }
                else return null;
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting value for key {key}. Exception: {e}", key, e.Message);
                return null;
            }
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
