using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN222_ApartmentManagement.Pages.FinancialReports;

[Authorize]
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.IsInRole("BQL_Manager"))
        {
            return RedirectToPage("/BQL_Manager/FinancialReports/Index");
        }

        return RedirectToPage("/Account/AccessDenied");
    }
}
