using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.FaceAuth;

[Authorize(Roles = "Resident")]
public class HistoryModel : PageModel
{
    private readonly IFaceAuthService _faceAuthService;

    public HistoryModel(IFaceAuthService faceAuthService)
    {
        _faceAuthService = faceAuthService;
    }

    public List<FaceAuthHistory> Histories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToPage("/Account/Login");
        }

        Histories = await _faceAuthService.GetHistoriesAsync(userId);
        return Page();
    }
}
