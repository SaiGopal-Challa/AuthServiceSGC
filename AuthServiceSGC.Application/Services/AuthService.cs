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
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly ISessionDetailsRepository _sessionDetailsRepository;

        public AuthService(IUserRepository userRepository, IRedisCacheProvider redisCacheService, ITokenBlacklistService tokenBlacklistService, ISessionDetailsRepository sessionDetailsRepository)
        {
            _userRepository = userRepository;
            _redisCacheService = redisCacheService;
            _tokenBlacklistService = tokenBlacklistService;
            _sessionDetailsRepository = sessionDetailsRepository;
        }

        public async Task<LoginResponseDTO> LoginUserAsync(LoginDTO loginDTO)
        {
            User user = new User();
            var clientIdConverter = new ConvertClientIDToLoginType();

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
            int SessionId = CreateSessionId.GetNewSessionId();
            // get logintype from clientid, call a common method to get it
            int LoginType = await clientIdConverter.GetLoginType(loginDTO.ClientID, loginDTO.Username);
            // if logintype is to send otp, don't generate token
            if (LoginType == 1)
            {
                return new LoginResponseDTO
                {
                    Success = true,
                    Message = "Login successful",
                    SessionId = SessionId,
                    LoginType = LoginType,
                    Token = null
                };
            }

            string token;

            // Generate JWT token if no otp is required
             token = TokenUtility.GenerateToken(user.Username, SessionId);

            // save the token using tokenrepository along with other details also store sessionId , model SessionAndOTPDTO
            
            // Return success response with token
            return new LoginResponseDTO
            {
                Success = true,
                Message = "Login successful",
                LoginType = LoginType,
                Token = token
            };
        }


        public async Task<LogoutResponseDTO> LogoutUserAsync(LogoutRequestDTO logoutRequestDTO)
        {
            try
            {
                var sessionDetails = new SessionsDetail
                {
                    SessionId = logoutRequestDTO.SessionID,
                    Token = logoutRequestDTO.Token
                };

                // Remove session details from JSON and database
                await _sessionDetailsRepository.RemoveSessionAndOTPFromJsonAsync(sessionDetails);
                // Uncomment when database implementation is ready
                // await _sessionDetailsRepository.RemoveSessionAndOTPFromPgSqlAsync(sessionDetails);

                // Remove session details from Redis JSON file
                await _redisCacheService.RemoveSessionAndOtpJsonAsync(logoutRequestDTO.SessionID, logoutRequestDTO.Token);

                // Add token to the blacklist
                await _tokenBlacklistService.AddToBlacklistFileAsync(logoutRequestDTO.Token);

                // Return success response
                return new LogoutResponseDTO
                {
                    Success = true,
                    Message = "User successfully logged out."
                };
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error during logout operation");

                // Return failure response
                return new LogoutResponseDTO
                {
                    Success = false,
                    Message = $"Logout failed: {ex.Message}"
                };
            }
        }
    }
}
