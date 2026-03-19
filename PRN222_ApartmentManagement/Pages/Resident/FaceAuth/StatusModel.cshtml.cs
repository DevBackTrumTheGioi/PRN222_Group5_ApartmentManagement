using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.FaceAuth;

[Authorize(Roles = "Resident")]
public class StatusModel : PageModel
{
    private readonly IFaceAuthService _faceAuthService;

    public StatusModel(IFaceAuthService faceAuthService)
    {
        _faceAuthService = faceAuthService;
    }

    public User Resident { get; set; } = null!;
    public List<FaceAuthHistory> RecentHistories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var user = await _faceAuthService.GetResidentByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        Resident = user;
        RecentHistories = await _faceAuthService.GetRecentHistoriesAsync(userId, 3);

        return Page();
    }
}
