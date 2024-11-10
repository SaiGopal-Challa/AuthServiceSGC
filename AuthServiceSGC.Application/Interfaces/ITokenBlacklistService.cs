using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Interfaces
{
    public interface ITokenBlacklistService
    {
        // Redis-based methods
        Task AddToBlacklistCacheAsync(string token);
        Task<bool> IsBlacklistedInCacheAsync(string token);

        // JSON file-based methods
        Task AddToBlacklistFileAsync(string token);
        bool IsBlacklistedInFile(string token);
    }
}
