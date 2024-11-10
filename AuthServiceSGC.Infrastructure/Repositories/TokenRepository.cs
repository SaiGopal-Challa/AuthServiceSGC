using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {

        // in place of this, i'm using SessionDetailsRepository

        // here i will add tokenBlacklist service

        public async Task<bool> AddTokenToBlacklist(string token)
        {
            bool status = false;
            //write logic to add the token to blacklist
            return true;
        }


    }

}
