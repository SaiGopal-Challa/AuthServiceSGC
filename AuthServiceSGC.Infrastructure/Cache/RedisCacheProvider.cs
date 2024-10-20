using AuthServiceSGC.Domain.Entities;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace AuthServiceSGC.Infrastructure.Cache
{
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private readonly IDatabase _redisDb;
        private readonly string _userReplicaFilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "RedisUserDataReplica.json");

        public RedisCacheProvider(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
            // Ensure the replica file exists
            if (!File.Exists(_userReplicaFilePath))
            {
                File.WriteAllText(_userReplicaFilePath, "[]"); // Initialize with empty array
            }
        }

        // Set a generic value in Redis
        public async Task SetAsync<T>(string key, T value)
        {
            var jsonData = System.Text.Json.JsonSerializer.Serialize(value);
            await _redisDb.StringSetAsync(key, jsonData, expiry: TimeSpan.FromMinutes(30));
        }

        // Get a generic value from Redis
        public async Task<T> GetAsync<T>(string key)
        {
            var data = await _redisDb.StringGetAsync(key);
            if (!data.HasValue)
                return default;

            return System.Text.Json.JsonSerializer.Deserialize<T>(data);
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


        // Check if the user exists in the JSON replica
        public async Task<bool> CheckUserExistsAsyncJson(string username)
        {
            var users = await GetReplicaUsersAsync();
            return users.Any(u => u.Username == username);
        }

        // Helper method to read the JSON file
        private async Task<List<User>> GetReplicaUsersAsync()
        {
            using (StreamReader reader = new StreamReader(_userReplicaFilePath))
            {
                string json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
        }

        // Add the user to the JSON replica
        public async Task AddUserAsyncJson(User user)
        {
            var users = await GetReplicaUsersAsync();
            users.Add(user);

            using (StreamWriter writer = new StreamWriter(_userReplicaFilePath, false))
            {
                string updatedJson = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented);
                await writer.WriteAsync(updatedJson);
            }
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