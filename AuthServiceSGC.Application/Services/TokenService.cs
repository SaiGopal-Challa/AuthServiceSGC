using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Services
{
    public class TokenService : ITokenService
    {
        public async Task<RefreshTokenDto> RefreshTokenAsync(RefreshTokenDto refreshToken)
        {
            
            if (string.IsNullOrEmpty(refreshToken.OldToken) || string.IsNullOrEmpty(refreshToken.SessionId))
            {
                refreshToken.Success = false;
                return refreshToken;
            }
            // check if token is valid , if valid, create new token & set success to true

            return refreshToken;
        }
    }

}
