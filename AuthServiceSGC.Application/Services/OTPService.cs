using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using AuthServiceSGC.Domain.Entities;
using AuthServiceSGC.Domain.Utilities;
using AuthServiceSGC.Infrastructure.Cache;
using AuthServiceSGC.Infrastructure.Repositories;
using AuthServiceSGC.Infrastructure.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace AuthServiceSGC.Application.Services
{
    public class OTPService: IOTPService
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ISessionDetailsRepository _sessionDetailsRepository;
        private readonly IRedisCacheProvider _redisCacheProvider;
        public OTPService(IEmailService emailService, IUserRepository userRepository, ISessionDetailsRepository sessionDetailsRepository, IRedisCacheProvider redisCacheProvider)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _sessionDetailsRepository = sessionDetailsRepository;
            _redisCacheProvider = redisCacheProvider;
        }

        
        public async Task<OTPResponseDTO> SendOTP(OTPRequestDTO oTPRequestDTO)
        {
            OTPResponseDTO oTPResponseDTO = new OTPResponseDTO();

            // Generate OTP
            string OTP = OTPUtility.GenerateOTP();

            // **Updated**: OTP Sending based on LoginType (e.g., email or SMS)
            if (oTPRequestDTO.LoginType == 1)
            {
                // Send OTP via email
                await SendOTPEmail(oTPRequestDTO.Username, OTP);
                oTPResponseDTO.Message = "OTP sent via email.";
            }
            else if (oTPRequestDTO.LoginType == 2)
            {
                // Send OTP via SMS (assuming SMS method exists)
                //await SendOTPSMS(OTP, oTPRequestDTO.Username);
                oTPResponseDTO.Message = "OTP sent via SMS.";
            }
            else
            {
                oTPResponseDTO.Message = "Invalid LoginType specified.";
                oTPResponseDTO.Success = false;
                return oTPResponseDTO;
            }

            // **Updated**: Session management for multiple sessions
            var existingSession = await _sessionDetailsRepository.GetSessionAndOTPFromJsonAsync(oTPRequestDTO.Username);

            if (existingSession != null)
            {
                // Increment session count and add new session detail
                existingSession.SessionCount += 1;
                existingSession.Sessions.Add(new SessionsDetail
                {
                    SessionId = oTPRequestDTO.SessionId,
                    OTP = OTP,
                    Token = null,
                    RefreshToken = null,
                    Expiry = DateTime.UtcNow.AddMinutes(5) // Setting expiry for OTP
                });
            }
            else
            {
                // Initialize a new session if none exists
                existingSession = new SessionAndOTPModel(oTPRequestDTO.Username)
                {
                    SessionCount = 1,
                    Sessions = new List<SessionsDetail>
            {
                new SessionsDetail
                {
                    SessionId = oTPRequestDTO.SessionId,
                    OTP = OTP,
                    Token = null,
                    RefreshToken = null,
                    Expiry = DateTime.UtcNow.AddMinutes(5) // Setting expiry for OTP
                }
            }
                };
            }

            // Save updated session details in JSON and cache
            await _sessionDetailsRepository.SaveSessionAndOTPJsonAsync(existingSession);
            await _redisCacheProvider.AddSessionAndOTPAsyncJson(existingSession);

            // **Updated**: Set response details
            oTPResponseDTO.Success = true;
            oTPResponseDTO.SessionID = oTPRequestDTO.SessionId;
            oTPResponseDTO.Token = null;

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

            // Fetch all session details for the username
            var sessionAndOtpModel = await _sessionDetailsRepository.GetSessionAndOTPFromJsonAsync(oTPValidateRequestDTO.Username);

            if (sessionAndOtpModel != null)
            {
                // Locate the session by SessionId
                var sessionDetail = sessionAndOtpModel.Sessions.FirstOrDefault(s => s.SessionId == oTPValidateRequestDTO.SessionId);

                if (sessionDetail != null)
                {
                    // Check if OTP matches
                    if (sessionDetail.OTP == oTPValidateRequestDTO.OTP)
                    {
                        // OTP is valid, generate the token
                        string token = TokenUtility.GenerateToken(oTPValidateRequestDTO.Username, oTPValidateRequestDTO.SessionId);

                        // Fill the response DTO with success information
                        oTPResponseDTO.Success = true;
                        oTPResponseDTO.Message = "OTP validation successful.";
                        oTPResponseDTO.SessionID = sessionDetail.SessionId;
                        oTPResponseDTO.Token = token;

                        // Update session details with the new token and reset OTP
                        sessionDetail.Token = token;
                        sessionDetail.OTP = null;
                        /*
                        
                            session is not being stored properly, multiple entries into json
                        
                        */
                        // Save updated session details in both JSON storage and Redis cache
                        await _sessionDetailsRepository.SaveSessionAndOTPJsonAsync(sessionAndOtpModel);
                        await _redisCacheProvider.AddSessionAndOTPAsyncJson(sessionAndOtpModel);
                    }
                    else
                    {
                        // OTP does not match
                        oTPResponseDTO.Success = false;
                        oTPResponseDTO.Message = "Invalid OTP.";
                    }
                }
                else
                {
                    // Session with provided SessionId does not exist
                    oTPResponseDTO.Success = false;
                    oTPResponseDTO.Message = "Session not found for the given SessionId.";
                }
            }
            else
            {
                // No session data found for the username
                oTPResponseDTO.Success = false;
                oTPResponseDTO.Message = "No session data found for the given username.";
            }

            return oTPResponseDTO;
        }


        //public async Task<OTPResponseDTO> SendOTP(OTPRequestDTO oTPRequestDTO)
        //{
        //    OTPResponseDTO oTPResponseDTO = new OTPResponseDTO();
        //    bool status = false;

        //    // Generate OTP
        //    string OTP = OTPUtility.GenerateOTP();

        //    if (oTPRequestDTO.LoginType == 1)
        //    {
        //        //send otp via email,
        //        await SendOTPEmail(OTP, oTPRequestDTO.Username);
        //    }

        //    var sessionAndOtpModel = new SessionAndOTPModel(oTPRequestDTO.Username)
        //    {
        //        SessionCount = 1,
        //        Sessions = new List<SessionsDetail>
        //        {
        //            new SessionsDetail
        //            {
        //                SessionId = oTPRequestDTO.SessionId,
        //                OTP = OTP,
        //                Token = null,
        //                RefreshToken = null,
        //                Expiry = null
        //            }
        //        }
        //    };
        //    //call SessionDetailsRepository to save the sessiondetails
        //    await _sessionDetailsRepository.SaveSessionAndOTPJsonAsync(sessionAndOtpModel);
        //    // call redisCacheService to save the sessiondetails


        //    return oTPResponseDTO;
        //}


        //public async Task<OTPResponseDTO> ValidateOTP(OTPValidateRequestDTO oTPValidateRequestDTO)
        //{
        //    OTPResponseDTO oTPResponseDTO = new OTPResponseDTO();

        //    // call SessionDetailsRepository to get the otp
        //    var result = await _sessionDetailsRepository.GetSessionAndOTPFromJsonAsync(oTPValidateRequestDTO.Username);

        //    var sessionAndOtpResponse = new SessionAndOTPModel(result.Username)
        //    {
        //        SessionCount = 1,
        //        Sessions = new List<SessionsDetail>
        //        {
        //            new SessionsDetail
        //            {
        //                SessionId = result.SessionId,
        //                OTP = result.OTP,
        //                Token = null,
        //                RefreshToken = null,
        //                Expiry = null
        //            }
        //        }
        //    };
        //    // Generate JWT token
        //    string token = TokenUtility.GenerateToken(oTPValidateRequestDTO.Username, oTPValidateRequestDTO.SessionId);

        //    //call SessionDetailsRepository to save the sessiondetails

        //    // call redisCacheService to save the sessiondetails

        //    return oTPResponseDTO;
        //}





    }
}
