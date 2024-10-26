using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.DTOs
{
    public class SessionAndOTPDTO
    {
        public string Username { get; set; }
        public int SessionCount { get; set; }
        public List<SessionDetail> Sessions { get; set; } = new List<SessionDetail>();

        public SessionAndOTPDTO(string username)
        {
            Username = username;
            SessionCount = 0; // Initialize session count
        }
    }

    public class SessionDetail
    {
        public int SessionId { get; set; }
        public string OTP { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiry { get; set; }
    }
}
