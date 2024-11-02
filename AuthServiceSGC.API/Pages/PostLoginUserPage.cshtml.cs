using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServiceSGC.API.Pages
{
    public class PostLoginUserPageModel : PageModel
    {
        //public string Username { get; set; } = "User"; // Default value if Username is missing
        //public string SessionID { get; set; }
        //public string Token { get; set; }


        public void OnGet()
        {
            //Username = HttpContext.Session.GetString("username") ?? "User";
            //SessionID = HttpContext.Session.GetString("sessionId");
            //Token = HttpContext.Session.GetString("token");
        }
    }
}
