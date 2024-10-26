using AuthServiceSGC.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Interfaces
{
    public interface IOTPService
    {
        public Task<OTPResponseDTO> SendOTP(OTPRequestDTO oTPRequestDTO);
        public Task<OTPResponseDTO> ValidateOTP(OTPValidateRequestDTO oTPValidateRequestDTO);
    }
}
