using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.FaceAuth;

[Authorize(Roles = "BQL_Manager")]
public class DashboardModel : PageModel
{
    private readonly IFaceAuthService _faceAuthService;

    public DashboardModel(IFaceAuthService faceAuthService)
    {
        _faceAuthService = faceAuthService;
    }

    [BindProperty(SupportsGet = true)]
    public int RecentDays { get; set; } = 30;

    public FaceAuthDashboardDto Dashboard { get; set; } = new();

    public async Task OnGetAsync()
    {
        Dashboard = await _faceAuthService.GetDashboardAsync(RecentDays);
    }
}
