using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using AuthServiceSGC.Domain.Entities;
using AuthServiceSGC.Domain.Utilities;
using AuthServiceSGC.Infrastructure.Cache;
using AuthServiceSGC.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRedisCacheProvider _redisCacheService;

        public AuthService(IUserRepository userRepository, IRedisCacheProvider redisCacheService)
        {
            _userRepository = userRepository;
            _redisCacheService = redisCacheService;
        }

        public async Task<LoginResponseDTO> LoginUserAsync(LoginDTO loginDTO)
        {
            User user = new User();
            // First, check if user exists in Redis replica (if using Redis for caching)
            //var user = await _redisCacheService.GetUserFromRedisAsync(loginDTO.Username);
            if (user == null)
            {
                // Fall back to JSON file if Redis data is not available
                
            }
            user = await _userRepository.GetUserFromJsonAsync(loginDTO.Username);
            if (user == null)
            {
                // Finally check in PostgreSQL if not found in Redis or JSON
                //user = await _userRepository.GetUserFromPostgresAsync(loginDTO.Username);
            }

            if (user == null)
            {
                return new LoginResponseDTO { Success = false, Message = "Invalid username or password." };
            }

            // Verify password
            if (!EncryptedPassword.Verify(loginDTO.Password, user.Password))
            {
                return new LoginResponseDTO { Success = false, Message = "Invalid username or password." };
            }

            // call CreateSessionId method to create a new sessionId and store it 

            // get logintype from clientid, call a common method to get it

            // if logintype is to send otp, call send otp method, don't generate token

            string token;

            // Generate JWT token if no otp is required
             token = TokenUtility.GenerateToken(user);

            // Return success response with token
            return new LoginResponseDTO
            {
                Success = true,
                Message = "Login successful",
                Token = token
            };
        }
    }
}
