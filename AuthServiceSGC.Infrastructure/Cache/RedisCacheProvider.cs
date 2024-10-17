using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Cache
{
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private readonly IDatabase _redisDb;

        public RedisCacheProvider(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        // Set a generic value in Redis
        public async Task SetAsync<T>(string key, T value)
        {
            var jsonData = JsonSerializer.Serialize(value);
            await _redisDb.StringSetAsync(key, jsonData, expiry: TimeSpan.FromMinutes(30));
        }

        // Get a generic value from Redis
        public async Task<T> GetAsync<T>(string key)
        {
            var data = await _redisDb.StringGetAsync(key);
            if (!data.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }

        // Checking if the user exists in Redis (user-specific)
        public async Task<bool> CheckUserExistsAsync(string username)
        {
            return await _redisDb.KeyExistsAsync(username);
        }

        public async Task SetUserAsync(string username)
        {
            // Storing the user in Redis
            await _redisDb.StringSetAsync(username, "exists", expiry: TimeSpan.FromMinutes(30)); // Expiration optional
        }
    }
}



/*namespace AuthServiceSGC.Infrastructure.Cache
{
    public class RedisCacheProvider
    {
        private readonly IDatabase _redisDb;

        public RedisCacheProvider(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task<bool> CheckUserExistsAsync(string username)
        {
            // Checking if the user exists in Redis
            return await _redisDb.KeyExistsAsync(username);
        }

        public async Task SetUserAsync(string username)
        {
            // Storing the user in Redis
            await _redisDb.StringSetAsync(username, "exists", expiry: TimeSpan.FromMinutes(30)); // Expiration optional
        }
    }
}
*/