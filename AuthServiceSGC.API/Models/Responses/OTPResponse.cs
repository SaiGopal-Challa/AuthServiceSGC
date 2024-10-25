namespace AuthServiceSGC.API.Models.Responses
{
    public class OTPResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? SessionID { get; set; } = null;
    }
}
