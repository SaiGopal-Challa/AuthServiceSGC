// AuthServiceSGC.API/Pages/Register.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AuthServiceSGC.Application.DTOs;
using AuthServiceSGC.Application.Interfaces;
using System.Threading.Tasks;

public class RegisterModel : PageModel
{
    private readonly IUserService _userService;

    public RegisterModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public UserRegisterDto UserRegisterDto { get; set; }

    public string Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _userService.RegisterUserAsync(UserRegisterDto);
        Message = result.Message;

        return Page();
    }
}
