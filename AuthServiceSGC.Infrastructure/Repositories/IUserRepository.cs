using AuthServiceSGC.Domain.Entities;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task AddUserAsyncJson(User user);
        Task AddUserAsync_Oracle(User user);

        // Add methods for fetching user
      
        Task<User> GetUserFromJsonAsync(string username);     // JSON
        Task<User> GetUserFromPostgresAsync(string username); // PostgreSQL
    }
}
