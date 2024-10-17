using AuthServiceSGC.Domain.Entities;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
    }
}
