using AuthServiceSGC.API.Models.Requests;
using AuthServiceSGC.API.Models.Responses;
using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new LoginResponse { Success = false, Message = "Invalid login request." });
            }

            try
            {
                // Create DTO for service layer
                var loginDto = new LoginDTO
                {
                    Username = loginRequest.Username,
                    Password = loginRequest.Password
                };

                // Service layer returns a DTO as well
                var result = await _authService.LoginUserAsync(loginDto);

                if (!result.Success)
                    return BadRequest(new LoginResponse { Message = result.Message, Success = false });

                return Ok(new LoginResponse { Message = "Login successful.", Success = true, Token = result.Token, LoginType = result.LoginType, SessionId = result.SessionId });
            }
            catch (Exception ex)
            {
                // need to log error
                Console.WriteLine($"Error in Login API: {ex.Message}");
                return StatusCode(500, new LoginResponse { Success = false, Message = "An error occurred while processing your request." });
            }
        }
    }
}
