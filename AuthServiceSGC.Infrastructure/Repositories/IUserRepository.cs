﻿using AuthServiceSGC.Domain.Entities;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task AddUserAsyncJson(User user);
        Task AddUserAsync_Oracle(User user);
    }
}
