﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Domain.Entities
{
    public class SessionAndOTPModel
    {
        public string Username { get; set; }
        public int SessionCount { get; set; }
        public List<SessionsDetail> Sessions { get; set; } = new List<SessionsDetail>();

        public SessionAndOTPModel(string username)
        {
            Username = username;
            SessionCount = 0; // Initialize session count
        }
    }
    public class SessionsDetail
    {
        public int? SessionId { get; set; }
        public string? OTP { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expiry { get; set; }
    }
}
