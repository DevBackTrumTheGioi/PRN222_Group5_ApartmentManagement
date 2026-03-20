using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Member.FinancialReports;

[Authorize(Roles = "BQT_Member")]
public class IndexModel : PageModel
{
    private readonly IFinancialReportService _financialReportService;

    public IndexModel(IFinancialReportService financialReportService)
    {
        _financialReportService = financialReportService;
    }

    public FinancialReportViewModel Report { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int Year { get; set; } = DateTime.Now.Year;

    [BindProperty(SupportsGet = true)]
    public int? Month { get; set; }

    public async Task OnGetAsync()
    {
        Report = await _financialReportService.GetReportAsync(Year, Month);
    }
}
