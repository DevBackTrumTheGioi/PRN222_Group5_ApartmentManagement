using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using PRN222_ApartmentManagement.Services.Interfaces;
using System.Security.Claims;

namespace PRN222_ApartmentManagement.Pages.Resident;

[Authorize(Roles = "Resident")]
public class IndexModel : PageModel
{
    private readonly IDashboardService _dashboardService;

    public IndexModel(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public ResidentDashboardViewModel DashboardData { get; set; } = null!;

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        DashboardData = await _dashboardService.GetResidentDashboardAsync(userId);
    }
}

