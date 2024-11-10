namespace AuthServiceSGC.API.Models.Requests
{
    public class LogoutRequest
    {
        public string Token { get; set; }
        public int SessionID { get; set; }
    }
}
