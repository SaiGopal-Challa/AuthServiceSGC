using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace AuthServiceSGC.API.Models.Responses
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } // JWT token (or other session-related token)

        [DefaultValue(1)]
        public int LoginType { get; set; }

        //[HiddenInput]
        public string? SessionID { get; set; }
        [HiddenInput]
        public string? PreferredName { get; set; }
    }
}
