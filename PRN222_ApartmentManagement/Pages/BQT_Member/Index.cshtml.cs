using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Member;

[Authorize(Roles = "BQT_Member")]
public class IndexModel : PageModel
{
    private readonly IDashboardService _dashboardService;

    public IndexModel(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public BQTMemberDashboardViewModel DashboardData { get; set; } = null!;

    public async Task OnGetAsync()
    {
        DashboardData = await _dashboardService.GetBQTMemberDashboardAsync();
    }
}

