using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AuthServiceSGC.Domain.Entities;
using System.Configuration;
using AuthServiceSGC.Domain.Constants;

namespace AuthServiceSGC.Domain.Utilities
{
    public static class TokenUtility
    {

        private static string SecretKey = AppsettingData.JWTSecretKey;

        public static string GenerateToken(string Username, int? SessionId )
        {
            if(SessionId == null) { SessionId = 1; }
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, Username),
                    new Claim(ClaimTypes.SerialNumber, SessionId.ToString()),
                    // Additional claims can be added here
                }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch
            {
                return null;
            }
        }
    }
}
