﻿using AuthServiceSGC.Domain.Entities;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace AuthServiceSGC.Infrastructure.Cache
{
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private readonly IDatabase _redisDb;
        private readonly string _userReplicaFilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "RedisUserDataReplica.json");
        private readonly string _sessionReplicaFilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "RedisSessionReplica.json");

        public RedisCacheProvider(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
            // Ensure the replica file exists
            if (!File.Exists(_userReplicaFilePath))
            {
                File.WriteAllText(_userReplicaFilePath, "[]"); // Initialize with empty array
            }
            if (!File.Exists(_sessionReplicaFilePath))
            {
                File.WriteAllText(_sessionReplicaFilePath, "[]"); // Initialize with empty array
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

        // Check if the user exists in the JSON replica
        public async Task<bool> CheckUserExistsAsyncJson(string username)
        {
            var users = await GetReplicaUsersAsync();
            return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
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

        public async Task<User> GetUserFromRedisAsync(string username)
        {
            var user = await _redisDb.StringGetAsync(username);
            if (!user.HasValue)
                return null;

            return JsonConvert.DeserializeObject<User>(user);
        }


        // Add SessionAndOTPModel details in JSON
        public async Task AddSessionAndOTPAsyncJson(SessionAndOTPModel sessionAndOtp)
        {
            var sessionData = await GetReplicaSessionsAsync();
            var existingSession = sessionData.FirstOrDefault(s => s.Username.Equals(sessionAndOtp.Username, StringComparison.OrdinalIgnoreCase));

            if (existingSession != null)
            {
                // Update the existing session details
                existingSession.SessionCount = sessionAndOtp.SessionCount;
                existingSession.Sessions = sessionAndOtp.Sessions;
            }
            else
            {
                // Add as a new session entry if none exists
                sessionData.Add(sessionAndOtp);
            }

            using (StreamWriter writer = new StreamWriter(_sessionReplicaFilePath, false))
            {
                string updatedJson = JsonConvert.SerializeObject(sessionData, Newtonsoft.Json.Formatting.Indented);
                await writer.WriteAsync(updatedJson);
            }
        }

        // Add SessionAndOTPModel details in Redis
        public async Task AddSessionAndOTPAsyncRedis(string key, SessionAndOTPModel sessionAndOtp)
        {
            var existingSessionData = await _redisDb.StringGetAsync(key);
            if (existingSessionData.HasValue)
            {
                // Deserialize existing session to update it
                var existingSession = JsonConvert.DeserializeObject<SessionAndOTPModel>(existingSessionData);
                existingSession.SessionCount = sessionAndOtp.SessionCount;
                existingSession.Sessions = sessionAndOtp.Sessions;

                // Serialize the updated session and set it in Redis
                var updatedJsonData = JsonConvert.SerializeObject(existingSession);
                await _redisDb.StringSetAsync(key, updatedJsonData, expiry: TimeSpan.FromMinutes(30));
            }
            else
            {
                // Add as a new session if it doesn't exist
                var jsonData = JsonConvert.SerializeObject(sessionAndOtp);
                await _redisDb.StringSetAsync(key, jsonData, expiry: TimeSpan.FromMinutes(30));
            }
        }

        // Retrieve SessionAndOTPModel details from JSON
        public async Task<SessionAndOTPModel> GetSessionAndOTPAsyncJson(string username)
        {
            var sessionData = await GetReplicaSessionsAsync();
            return sessionData.FirstOrDefault(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        // Retrieve SessionAndOTPModel details from Redis
        public async Task<SessionAndOTPModel> GetSessionAndOTPAsyncRedis(string key)
        {
            var sessionData = await _redisDb.StringGetAsync(key);
            if (!sessionData.HasValue)
                return null;

            return JsonConvert.DeserializeObject<SessionAndOTPModel>(sessionData);
        }

        // Helper method to read SessionAndOTPModel list from JSON file
        private async Task<List<SessionAndOTPModel>> GetReplicaSessionsAsync()
        {
            using (StreamReader reader = new StreamReader(_sessionReplicaFilePath))
            {
                string json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<SessionAndOTPModel>>(json) ?? new List<SessionAndOTPModel>();
            }
        }

        public async Task RemoveSessionAndOtpJsonAsync(int sessionId, string token)
        {
            // Retrieve the session data from the replica JSON
            var sessionData = await GetReplicaSessionsAsync();

            // Iterate over each session and filter out matching session IDs and tokens
            foreach (var session in sessionData.ToList()) // Use ToList() to allow modifications during iteration
            {
                var sessionsList = session.Sessions.ToList();
                sessionsList.RemoveAll(sd => sd.SessionId == sessionId && sd.Token == token);

                if (sessionsList.Any())
                {
                    // If any sessions remain, update the session data
                    session.Sessions = sessionsList;
                }
                else
                {
                    // Remove the session object completely if no sessions are left
                    sessionData.Remove(session);
                }
            }

            // Write the updated session data back to the JSON file
            try
            {
                using (StreamWriter writer = new StreamWriter(_sessionReplicaFilePath, false))
                {
                    string updatedJson = JsonConvert.SerializeObject(sessionData, Newtonsoft.Json.Formatting.Indented);
                    await writer.WriteAsync(updatedJson);
                }
            }
            catch (IOException ex)
            {
                throw new Exception("Failed to update session replica file.", ex);
            }
        }



        public async Task RemoveSessionAndOtpRedisAsync(string redisKey, int sessionId, string token)
        {
            try
            {
                var sessionData = await _redisDb.StringGetAsync(redisKey);

                if (sessionData.HasValue)
                {
                    // Deserialize the session data
                    var sessionModel = JsonConvert.DeserializeObject<SessionAndOTPModel>(sessionData);

                    // Remove the matching session
                    sessionModel?.Sessions?.RemoveAll(sd => sd.SessionId == sessionId && sd.Token == token);

                    if (sessionModel?.Sessions?.Any() == true)
                    {
                        // If sessions remain, update the Redis cache with the filtered session data
                        var updatedJson = JsonConvert.SerializeObject(sessionModel);
                        await _redisDb.StringSetAsync(redisKey, updatedJson, expiry: TimeSpan.FromMinutes(30));
                    }
                    else
                    {
                        // If no sessions remain, remove the key from Redis
                        await _redisDb.KeyDeleteAsync(redisKey);
                    }
                }
            }
            catch (RedisException ex)
            {
                throw new Exception($"Failed to interact with Redis for key: {redisKey}", ex);
            }
        }


        // === Redis Cache-Based Methods ===

        // Method for adding token to Redis Cache (can be commented out when Redis is not in use)
        public async Task AddToBlacklistCacheAsync(string token)
        {
            await _redisDb.StringSetAsync(token, "blacklisted");
        }

        // Method to check if a token is blacklisted in Redis (can be commented out when Redis is not in use)
        public async Task<bool> IsBlacklistedInCacheAsync(string token)
        {
            var cacheValue = await _redisDb.StringGetAsync(token);
            return cacheValue == "blacklisted";
        }
    }
}

/*
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
 */

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




/*
 // Add SessionAndOTPModel details in redisjson
        public async Task AddSessionAndOTPAsyncJson(SessionAndOTPModel sessionAndOtp)
        {
            var sessionData = await GetReplicaSessionsAsync();
            sessionData.Add(sessionAndOtp);

            using (StreamWriter writer = new StreamWriter(_sessionReplicaFilePath, false))
            {
                string updatedJson = JsonConvert.SerializeObject(sessionData, Newtonsoft.Json.Formatting.Indented);
                await writer.WriteAsync(updatedJson);
            }
        }

        // Add SessionAndOTPModel details in Redis
        public async Task AddSessionAndOTPAsyncRedis(string key, SessionAndOTPModel sessionAndOtp)
        {
            var jsonData = JsonConvert.SerializeObject(sessionAndOtp);
            await _redisDb.StringSetAsync(key, jsonData, expiry: TimeSpan.FromMinutes(30));
        }

        // Retrieve SessionAndOTPModel details from JSON
        public async Task<SessionAndOTPModel> GetSessionAndOTPAsyncJson(string username)
        {
            var sessionData = await GetReplicaSessionsAsync();
            return sessionData.FirstOrDefault(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        // Retrieve SessionAndOTPModel details from Redis
        public async Task<SessionAndOTPModel> GetSessionAndOTPAsyncRedis(string key)
        {
            var sessionData = await _redisDb.StringGetAsync(key);
            if (!sessionData.HasValue)
                return null;

            return JsonConvert.DeserializeObject<SessionAndOTPModel>(sessionData);
        }

        // Helper method to read SessionAndOTPModel list from JSON file
        private async Task<List<SessionAndOTPModel>> GetReplicaSessionsAsync()
        {
            using (StreamReader reader = new StreamReader(_sessionReplicaFilePath))
            {
                string json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<SessionAndOTPModel>>(json) ?? new List<SessionAndOTPModel>();
            }
        }
 
 */