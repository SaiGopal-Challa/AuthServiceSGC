﻿namespace AuthServiceSGC.API.Models.Requests
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string? ClientID { get; set; }
    }
}
