using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.FamilyMembers;

[Authorize(Roles = "Resident")]
public class DetailsModel : PageModel
{
    private readonly IUserManagementService _userService;

    public DetailsModel(IUserManagementService userService)
    {
        _userService = userService;
    }

    public ApartmentDetailViewModel? Apartment { get; set; }
    public List<ApartmentResidentDto> Residents { get; set; } = new();
    public ResidencyType? CurrentUserResidencyType { get; set; }

    public async Task<IActionResult> OnGetAsync(int apartmentId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return RedirectToPage("/Account/Login");

        Apartment = await _userService.GetApartmentWithDetailsAsync(apartmentId, userId.Value);
        if (Apartment == null)
        {
            TempData["ErrorMessage"] = "Bạn không có quyền truy cập căn hộ này.";
            return RedirectToPage("./Index");
        }

        Residents = await _userService.GetAllResidentsForApartmentAsync(apartmentId);
        CurrentUserResidencyType = Residents.FirstOrDefault(r => r.UserId == userId)?.ResidencyType;
        return Page();
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }
}
