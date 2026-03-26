using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.FamilyMembers;

[Authorize(Roles = "Resident")]
public class IndexModel : PageModel
{
    private readonly IUserManagementService _userService;

    public IndexModel(IUserManagementService userService)
    {
        _userService = userService;
    }

    public List<MyApartmentViewModel> Apartments { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return RedirectToPage("/Account/Login");

        Apartments = await _userService.GetMyApartmentsAsync(userId.Value);
        return Page();
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }
}
