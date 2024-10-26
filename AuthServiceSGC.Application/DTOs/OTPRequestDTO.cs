using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.DTOs
{
    public class OTPRequestDTO
    {
        public string Username { get; set; }
        public int? LoginType { get; set; }
        public int? SessionId { get; set; }
    }

    public class OTPValidateRequestDTO
    {
        public string Username { get; set; }
        public string OTP { get; set; }
        public int? SessionId { get; set; }
    }
}
