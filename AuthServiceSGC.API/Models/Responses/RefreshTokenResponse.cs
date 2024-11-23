namespace AuthServiceSGC.API.Models.Responses
{
    public class RefreshTokenResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string SessionId { get; set; }
    }
}
