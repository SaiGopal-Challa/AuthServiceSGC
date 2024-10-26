using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.DTOs
{
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }  // JWT Token or Session Token

        [DefaultValue(1)]
        public int LoginType { get; set; }

        public string? SessionId { get; set; }
    }
}
