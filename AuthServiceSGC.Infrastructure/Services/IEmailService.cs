using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Services
{
    public interface IEmailService
    {
        Task<EmailResponseDTO> SendOtpEmailAsync(string toEmail, string otp);
    }
}
