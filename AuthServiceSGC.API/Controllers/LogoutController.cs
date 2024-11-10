using AuthServiceSGC.API.Models.Requests;
using AuthServiceSGC.API.Models.Responses;
using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using AuthServiceSGC.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    [Route("api/Logout")]
    public class LogoutController: ControllerBase
    {
        private readonly IAuthService _authService;

        public LogoutController(IAuthService authService)
        {
            _authService = authService;
        }

        // get username, current token, sessionId,
        // 
        [HttpPost]
        [Route("LogoutUser")]
        //[Authorize]
        public async Task<ActionResult<LogoutRequest>> LogoutUser(LogoutRequest logoutRequest)
        {
            if (logoutRequest.SessionID == null || logoutRequest.Token ==null) 
            { return BadRequest(new LogoutResponse { Success = false, Message = "Invalid logout request." }); }

            var logoutRequestDTO = new LogoutRequestDTO
            {
                SessionID = logoutRequest.SessionID,
                Token = logoutRequest.Token
            };

            var result = await _authService.LogoutUserAsync(logoutRequestDTO);

            if (result.Success)
            {
                //HttpContext.Session.Remove("Username");
                //HttpContext.Session.Remove("SessionID");
                //HttpContext.Session.Remove("Token");
                return Ok(new LogoutResponse { Success = true, Message = result.Message });
            }
            else
            {
                return BadRequest(new LogoutResponse { Success = false, Message = "Faced issue while logging out" });
            }
            return null;
        }

        // ALSO ADD BELOW THING TO REMOVE SESSION DETAILS FROM PAGE
        //HttpContext.Session.Remove("Username");
        //HttpContext.Session.Remove("SessionID");
        //HttpContext.Session.Remove("Token");
    }
}
