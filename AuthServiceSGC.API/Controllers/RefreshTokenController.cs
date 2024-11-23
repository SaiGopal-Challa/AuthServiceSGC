using AuthServiceSGC.API.Models.Requests;
using AuthServiceSGC.API.Models.Responses;
using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    [Route("api/RefreshToken")]
    public class RefreshTokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public RefreshTokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("GetNewToken")]
        public async Task<ActionResult<RefreshTokenResponse>> NewToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest == null || string.IsNullOrEmpty(refreshTokenRequest.Token) || string.IsNullOrEmpty(refreshTokenRequest.SessionId))
            {
                return BadRequest(new RefreshTokenResponse { Success = false, Message = "Invalid refresh token request." });
            }

            try
            {
                // Create DTO for service layer
                var refreshTokenDto = new RefreshTokenDto
                {
                    OldToken = refreshTokenRequest.Token,
                    SessionId = refreshTokenRequest.SessionId
                };

                // Service layer returns a DTO as well
                var result = await _tokenService.RefreshTokenAsync(refreshTokenDto);

                if (!result.Success)
                    return BadRequest(new RefreshTokenResponse { Message = "Token refreshed failed", Success = false });

                return Ok(new RefreshTokenResponse { Message = "Token refreshed successfully.", Success = true, Token = result.NewToken, SessionId = result.SessionId });
            }
            catch (Exception ex)
            {
                // need to log error
                Console.WriteLine($"Error in RefreshToken API: {ex.Message}");
                return StatusCode(500, new RefreshTokenResponse { Success = false, Message = "An error occurred while processing your request." });
            }
        }
    }
}
