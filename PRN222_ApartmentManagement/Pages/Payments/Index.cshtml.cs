using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN222_ApartmentManagement.Pages.Payments;

[Authorize]
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.IsInRole("BQL_Manager"))
        {
            return RedirectToPage("/BQL_Manager/Payments/Index");
        }

        if (User.IsInRole("BQL_Staff"))
        {
            return RedirectToPage("/BQL_Staff/Payments/Index");
        }

        if (User.IsInRole("Resident"))
        {
            return RedirectToPage("/Resident/Payments/Index");
        }

        return RedirectToPage("/Account/AccessDenied");
    }
}
