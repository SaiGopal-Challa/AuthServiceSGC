namespace AuthServiceSGC.API.Models.Requests
{
    public class OTPRequest
    {
        public string Username { get; set; }
        public string? LoginType { get; set; }
        public string? SessionId { get; set; }
    }
}
