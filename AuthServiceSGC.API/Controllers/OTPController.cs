using AuthServiceSGC.API.Models.Requests;
using AuthServiceSGC.API.Models.Responses;
using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using AuthServiceSGC.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    [Route("api/OTP")]
    public class OTPController : ControllerBase
    {
        private readonly IOTPService _otpService;

        public OTPController(IOTPService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost]
        [Route("ValidateOTP")]
        public async Task<IActionResult> ValidateOTP([FromBody] OTPValidateRequest otpRequest)
        {
            OTPResponse otpResponse = new OTPResponse();
            try
            {
                // create OTPRequestDTO object and pass it to OTPService
                var otpDTO = new OTPValidateRequestDTO { Username = otpRequest.Username, SessionId = otpRequest.SessionId, OTP = otpRequest.OTP };

                var result = await _otpService.ValidateOTP(otpDTO);

                otpResponse.Success = result.Success;
                otpResponse.Message = result.Message;
                otpResponse.SessionID = result.SessionID;
                otpResponse.Token = result.Token;

                return Ok(otpResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                otpResponse.Success = false;
                otpResponse.Message = "An error occurred while validating OTP.";
                return StatusCode(500, otpResponse);
            }
        }


        [HttpPost]
        [Route("SendOTP")]
        public async Task<ActionResult<OTPResponse>> SendOTP([FromBody] OTPRequest otpRequest)
        {
            OTPResponse otpResponse = new OTPResponse();
            try
            {
                // create OTPRequestDTO object and pass it to OTPService
                var otpDTO = new OTPRequestDTO { Username = otpRequest.Username, SessionId = otpRequest.SessionId, LoginType = otpRequest.LoginType };

                var result = await _otpService.SendOTP(otpDTO);

                otpResponse.Success = result.Success;
                otpResponse.Message = result.Message;
                otpResponse.Token = result.Token;

                return Ok(otpResponse);
            }
            catch(Exception ex)
            {
                // Log the exception (ex)
                otpResponse.Success = false;
                otpResponse.Message = "An error occurred while sending OTP.";
                return StatusCode(500, otpResponse);
            }
        }
    }
}
