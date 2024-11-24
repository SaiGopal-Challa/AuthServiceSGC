using AuthServiceSGC.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    public class RLMiddlewareFlagController : ControllerBase
    {
        private readonly RLMiddlewareFlagService _rLMiddlewareFlagService;
        public RLMiddlewareFlagController(RLMiddlewareFlagService rLMiddlewareFlagService)
        {
            _rLMiddlewareFlagService = rLMiddlewareFlagService;
        }

        [HttpPost]
        [Authorize]
        [Route("api/RLMiddlewareFlagController/UpdateFlag")]
        public IActionResult UpdateFlag([FromBody] UpdateFlagInputModel updateFlagInputModel)
        {
            // implement further authorization 
            // we can set a key value pair in redis (or other db), and take value as input and check to authorize
            string status = _rLMiddlewareFlagService.UpdateFlag(updateFlagInputModel);
            if (string.IsNullOrEmpty(status))
            {
                return Ok(new { Success = true, Message = "Flag updated successfully." });
            }
            else
            {
                return BadRequest(status);
            }
        }
    }
}
