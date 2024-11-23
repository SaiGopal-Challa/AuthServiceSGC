namespace AuthServiceSGC.API.Models.Requests
{
    public class UpdateAccountRequest
    {
    }

    public class DeleteAccountRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
