namespace AuthServiceSGC.API.Models.Requests
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }
        public string SessionId { get; set; }
    }
}
