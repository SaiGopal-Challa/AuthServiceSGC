using AuthServiceSGC.API.Models.Requests;
using AuthServiceSGC.API.Models.Responses;
using AuthServiceSGC.Application.Interfaces;
using AuthServiceSGC.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Models.Responses.RegisterResponse>> Register([FromBody] RegisterRequest registerRequest)
        {
            var userRegisterDto = new UserRegisterDto
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber
            };

            var result = await _userService.RegisterUserAsync(userRegisterDto);

            var response = new Models.Responses.RegisterResponse
            {
                Success = result.Success,
                Message = result.Message
            };

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }
    }
}
