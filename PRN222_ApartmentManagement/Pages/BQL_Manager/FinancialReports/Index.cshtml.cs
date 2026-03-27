using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.FinancialReports;

[Authorize(Policy = "AdminAndBQLManager")]
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

    public async Task<IActionResult> OnPostExportAsync()
    {
        var content = await _financialReportService.ExportExcelAsync(Year, Month);
        var fileName = Month.HasValue
            ? $"bao-cao-tai-chinh-{Year}-{Month.Value:D2}.xls"
            : $"bao-cao-tai-chinh-{Year}.xls";

        return File(content, "application/vnd.ms-excel; charset=utf-8", fileName);
    }
}
