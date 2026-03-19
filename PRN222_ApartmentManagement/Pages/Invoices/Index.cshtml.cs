using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN222_ApartmentManagement.Pages.Invoices;

[Authorize]
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.IsInRole("BQL_Manager"))
        {
            return RedirectToPage("/BQL_Manager/Invoices/Index");
        }

        if (User.IsInRole("BQL_Staff"))
        {
            return RedirectToPage("/BQL_Staff/Invoices/Index");
        }

        if (User.IsInRole("Resident"))
        {
            return RedirectToPage("/Resident/Invoices/Index");
        }

        return RedirectToPage("/Account/AccessDenied");
    }
}
