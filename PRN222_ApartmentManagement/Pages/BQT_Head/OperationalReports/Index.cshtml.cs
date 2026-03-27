using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Head.OperationalReports;

[Authorize(Roles = "BQT_Head")]
public class IndexModel : PageModel
{
    private readonly IOperationalReportService _operationalReportService;

    public IndexModel(IOperationalReportService operationalReportService)
    {
        _operationalReportService = operationalReportService;
    }

    [BindProperty(SupportsGet = true)]
    public OperationalReportFilterDto Filter { get; set; } = new();

    public OperationalReportDto Report { get; set; } = new();

    public async Task OnGetAsync()
    {
        Report = await _operationalReportService.GetOperationalReportAsync(Filter);
    }
}
