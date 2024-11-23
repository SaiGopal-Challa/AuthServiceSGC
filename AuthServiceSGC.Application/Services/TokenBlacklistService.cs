using AuthServiceSGC.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly string _blacklistFilePath;
        private readonly ConcurrentDictionary<string, DateTime> _blacklistCache;

        public TokenBlacklistService(IDistributedCache cache)
        {
            _blacklistFilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "TokenBlacklist.json");
            _blacklistCache = LoadBlacklistFromFile(); // Load JSON data into cache on initialization
        }

        // === Redis Cache-Based Methods ===

        // Method for adding token to Redis Cache (can be commented out when Redis is not in use)
        public async Task AddToBlacklistCacheAsync(string token)
        {
        }

        // Method to check if a token is blacklisted in Redis (can be commented out when Redis is not in use)
        public async Task<bool> IsBlacklistedInCacheAsync(string token)
        {
            return true;
        }

        // === JSON File-Based Methods ===

        // Method for adding token to blacklist in JSON (file-based only)
        public async Task AddToBlacklistFileAsync(string token)
        {
            var expirationTime = GetTokenExpiration(token);
            if (expirationTime.HasValue)
            {
                // Update in-memory cache and persist to JSON
                _blacklistCache[token] = expirationTime.Value;
                await SaveBlacklistToFileAsync();
            }
        }

        // Method to check if a token is blacklisted in JSON (file-based only)
        public bool IsBlacklistedInFile(string token)
        {
            DateTime expirationTime = default;
            if (_blacklistCache.TryGetValue(token, out expirationTime))
            {
                if (expirationTime < DateTime.UtcNow)
                {
                    _blacklistCache.TryRemove(token, out _);
                    SaveBlacklistToFileAsync().Wait();
                    return false; // Token has expired
                }
                return true; // Token is still valid
            }
            return false; // Token not found in JSON
        }

        // === Helper Methods ===

        // Method to load blacklist data from JSON file (file-based only)
        private ConcurrentDictionary<string, DateTime> LoadBlacklistFromFile()
        {
            if (!File.Exists(_blacklistFilePath))
                return new ConcurrentDictionary<string, DateTime>();

            var jsonData = File.ReadAllText(_blacklistFilePath);

            // Deserialize to a dictionary first, then convert to a ConcurrentDictionary
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, DateTime>>(jsonData);
            return new ConcurrentDictionary<string, DateTime>(dictionary ?? new Dictionary<string, DateTime>());
        }

        // Method to save blacklist data back to the JSON file (file-based only)
        private async Task SaveBlacklistToFileAsync()
        {
            var jsonData = JsonSerializer.Serialize(_blacklistCache);
            await File.WriteAllTextAsync(_blacklistFilePath, jsonData);
        }

        // Method to extract the expiration time of a token (from JWT)
        private DateTime? GetTokenExpiration(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return jwtToken?.ValidTo;
        }

        public async Task ClearBlackListJsonFile() 
        {
            
        }
    }
}
