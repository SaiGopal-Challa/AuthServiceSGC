namespace AuthServiceSGC.API.Models.Requests
{
    public class OTPRequest
    {
        public string Username { get; set; }
        public int? LoginType { get; set; }
        public int? SessionId { get; set; }
    }

    public class OTPValidateRequest
    {
        public string Username { get; set; }
        public string OTP { get; set; }
        public int? SessionId { get; set; }
    }
}
