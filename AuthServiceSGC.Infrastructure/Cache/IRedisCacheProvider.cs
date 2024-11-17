using AuthServiceSGC.Domain.Entities;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Cache
{
    public interface IRedisCacheProvider
    {
        Task SetAsync<T>(string key, T value);
        Task<T> GetAsync<T>(string key);
        Task<bool> CheckUserExistsAsync(string username); // user-specific method

        public Task SetUserAsync(string username);

        Task<bool> CheckUserExistsAsyncJson(string username);
        public Task AddUserAsyncJson(User user);

        Task<User> GetUserFromRedisAsync(string username);


        public Task AddSessionAndOTPAsyncJson(SessionAndOTPModel sessionAndOtp);

        public Task RemoveSessionAndOtpJsonAsync(int sessionId, string token);
        Task AddToBlacklistCacheAsync(string token);
        Task<bool> IsBlacklistedInCacheAsync(string token);
    }
}
