using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using AuthServiceSGC.Domain.Entities;
using AuthServiceSGC.Domain.Utilities;
using AuthServiceSGC.Infrastructure.Repositories;
using AuthServiceSGC.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Services
{
    public class OTPService: IOTPService
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        public OTPService(IEmailService emailService, IUserRepository userRepository)
        {
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<OTPResponseDTO> SendOTP(OTPRequestDTO oTPRequestDTO)
        {
            OTPResponseDTO oTPResponseDTO = new OTPResponseDTO();
            bool status = false;

            // Generate OTP
            string OTP = OTPUtility.GenerateOTP();

            if (oTPRequestDTO.LoginType == 1)
            {
                //send otp via email,

            }

            //call SessionDetailsRepository to save the sessiondetails

            // call redisCacheService to save the sessiondetails


            return oTPResponseDTO;
        }

        // write send otp via mobile, call send SMSService

        //  call send EmailService , SendOtpEmailAsync(string toEmail, string otp)
        public async Task SendOTPEmail(string Username, string OTP)
        {
             (string EmailId, string MobileNumber) = await _userRepository.GetUserContactFromJsonAsync(Username);

            EmailResponseDTO emailResponseDTO = await _emailService.SendOtpEmailAsync(EmailId, OTP);
        }

        public async Task<OTPResponseDTO> ValidateOTP(OTPValidateRequestDTO oTPValidateRequestDTO)
        {
            OTPResponseDTO oTPResponseDTO = new OTPResponseDTO();
            // call SessionDetailsRepository to validate the otp


            // Generate JWT token
            string token = TokenUtility.GenerateToken(oTPValidateRequestDTO.Username, oTPValidateRequestDTO.SessionId);

            //call SessionDetailsRepository to save the sessiondetails

            // call redisCacheService to save the sessiondetails

            return oTPResponseDTO;
        }





    }
}
