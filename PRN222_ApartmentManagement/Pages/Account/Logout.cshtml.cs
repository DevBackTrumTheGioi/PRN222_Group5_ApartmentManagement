using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN222_ApartmentManagement.Pages.Account;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        
        // Remove JWT cookie if it exists
        if (Request.Cookies.ContainsKey("AuthToken"))
        {
            Response.Cookies.Delete("AuthToken");
        }

        return RedirectToPage("/Account/Login");
    }
}

