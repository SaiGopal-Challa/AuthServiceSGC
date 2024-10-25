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
        public string? LoginType { get; set; }
        public string? SessionId { get; set; }
    }
}
