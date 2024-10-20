using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using AuthServiceSGC.Infrastructure.Cache;
using AuthServiceSGC.Infrastructure.Repositories;
using AuthServiceSGC.Domain.Entities;
using System.Threading.Tasks;
using AuthServiceSGC.Domain.Utilities;

namespace AuthServiceSGC.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRedisCacheProvider _redisCacheProvider;

        public UserService(IUserRepository userRepository, IRedisCacheProvider redisCacheProvider)
        {
            _userRepository = userRepository;
            _redisCacheProvider = redisCacheProvider;
        }

        public async Task<RegisterResultDto> RegisterUserAsync(UserRegisterDto userRegisterDto)
        {
            // Check if user already exists in Redis
            //bool userExistsInCache = await _redisCacheProvider.CheckUserExistsAsync(userRegisterDto.Username);
            
            // Call CheckUserExistsAsyncJson to check if user exists in the JSON file
            bool userExistsInJson = await _redisCacheProvider.CheckUserExistsAsyncJson(userRegisterDto.Username);

            if (userExistsInJson)
            //if (userExistsInCache)
            {
                return new RegisterResultDto
                {
                    Success = false,
                    Message = "User already exists (Cached)"
                };
            }

            // Store user in Oracle SQL DB
            var user = new User
            {
                Username = userRegisterDto.Username,
                Password = EncryptedPassword.Hash(userRegisterDto.Password), // Assuming EncryptedPassword is in Domain Layer
                Email = userRegisterDto.Email,
                PhoneNumber = userRegisterDto.PhoneNumber
            };

            await _userRepository.AddUserAsync(user);

            // Set user in Redis Cache
            //await _redisCacheProvider.SetUserAsync(userRegisterDto.Username);

            // Add user to JSON file as well
            await _redisCacheProvider.AddUserAsyncJson(user);

            return new RegisterResultDto
            {
                Success = true,
                Message = "User registered successfully"
            };
        }
    }
}
