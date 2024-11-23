using AuthServiceSGC.Application.DTOs;

namespace AuthServiceSGC.Application.Interfaces
{
    public interface ITokenService
    {
        public Task<RefreshTokenDto> RefreshTokenAsync(RefreshTokenDto refreshToken);
    }
}