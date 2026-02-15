using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Account/Login");
        }

        var role = User.FindFirstValue(ClaimTypes.Role);

        return role switch
        {
            nameof(UserRole.Admin) => RedirectToPage("/Admin/Index"),
            nameof(UserRole.BQL_Manager) => RedirectToPage("/BQL_Manager/Index"),
            nameof(UserRole.BQL_Staff) => RedirectToPage("/BQL_Staff/Index"),
            nameof(UserRole.Resident) => RedirectToPage("/Resident/Index"),
            nameof(UserRole.BQT_Head) => RedirectToPage("/BQT_Head/Index"),
            nameof(UserRole.BQT_Member) => RedirectToPage("/BQT_Member/Index"),
            _ => RedirectToPage("/Account/Login")
        };
    }
}

