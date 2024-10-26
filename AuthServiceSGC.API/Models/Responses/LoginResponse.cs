using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace AuthServiceSGC.API.Models.Responses
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } // JWT token (or other session-related token)

        
        public int LoginType { get; set; } = 1;

        //[HiddenInput]
        public string? SessionId { get; set; }
        [HiddenInput]
        public string? PreferredName { get; set; }
    }
}
