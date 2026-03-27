using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;
namespace PRN222_ApartmentManagement.Pages.BQT_Head.FinancialReports;

[Authorize(Roles = "BQT_Head")]
public class IndexModel : PageModel
{
    public FinancialReportViewModel Report { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int Year { get; set; } = DateTime.Now.Year;

    [BindProperty(SupportsGet = true)]
    public int? Month { get; set; }

    public IActionResult OnGet()
    {
        return RedirectToPage("/Account/AccessDenied");
    }
}
