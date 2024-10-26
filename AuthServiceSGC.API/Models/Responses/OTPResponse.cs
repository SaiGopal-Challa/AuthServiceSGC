namespace AuthServiceSGC.API.Models.Responses
{
    public class OTPResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? SessionID { get; set; } = null;

        public string? Token { get; set; }
    }
}
