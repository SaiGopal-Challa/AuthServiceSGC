namespace AuthServiceSGC.API.Models.Requests
{
    public class LogoutRequest
    {

        public string Username { get; set; }

        public string? SessionID { get; set; }
    }
}
