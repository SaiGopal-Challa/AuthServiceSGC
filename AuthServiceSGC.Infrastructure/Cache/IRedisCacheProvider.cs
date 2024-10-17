using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Cache
{
    public interface IRedisCacheProvider
    {
        Task SetAsync<T>(string key, T value);
        Task<T> GetAsync<T>(string key);
        Task<bool> CheckUserExistsAsync(string username); // user-specific method

        public Task SetUserAsync(string username);
    }
}
