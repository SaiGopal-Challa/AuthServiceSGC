using AuthServiceSGC.API.Models.Requests;
using AuthServiceSGC.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class OTPController : ControllerBase
    {
        [HttpPost]
        [Route("SendOTP")]
        public async Task<OTPResponse> SendOTP([FromBody] OTPRequest otpRequest)
        {
            OTPResponse otpResponse = new OTPResponse();
            try
            {
                // create OTPRequestDTO object and pass it to service
                // Send OTP to the user
                return otpResponse;
            }
            catch
            {
                return otpResponse;
            }
        }
    }
}
