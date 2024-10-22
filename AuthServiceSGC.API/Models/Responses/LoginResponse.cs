namespace AuthServiceSGC.API.Models.Responses
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } // JWT token (or other session-related token)
    }
}
