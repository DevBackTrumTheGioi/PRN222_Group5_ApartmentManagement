using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.FaceAuth;

[Authorize(Roles = "BQL_Staff")]
public class IndexModel : PageModel
{
    private readonly IFaceAuthService _faceAuthService;

    public IndexModel(IFaceAuthService faceAuthService)
    {
        _faceAuthService = faceAuthService;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsRegistered { get; set; }

    public List<FaceResidentSummaryDto> Residents { get; set; } = new();
    public FaceAuthDashboardDto Summary { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Residents = await _faceAuthService.GetResidentSummariesAsync(SearchQuery, IsRegistered);
        Summary = await _faceAuthService.GetDashboardAsync();
    }

    public async Task<IActionResult> OnPostResetAsync(int residentId)
    {
        var actorUserId = GetCurrentUserId();
        if (actorUserId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var result = await _faceAuthService.ResetFaceAsync(actorUserId.Value, residentId);
        if (result.Success)
        {
            SuccessMessage = "Đã xóa dữ liệu Face ID của cư dân.";
        }
        else
        {
            ErrorMessage = result.ErrorMessage ?? "Không thể xóa dữ liệu Face ID.";
        }

        return RedirectToPage(new { SearchQuery, IsRegistered });
    }

    private int? GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdString, out var userId) ? userId : null;
    }
}
