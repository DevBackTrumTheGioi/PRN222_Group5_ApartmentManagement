using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.FamilyMembers;

[Authorize(Roles = "Resident")]
public class AddFamilyMemberModel : PageModel
{
    private readonly IUserManagementService _userService;

    public AddFamilyMemberModel(IUserManagementService userService)
    {
        _userService = userService;
    }

    public int ApartmentId { get; set; }
    public string ApartmentNumber { get; set; } = "";

    [BindProperty]
    public FamilyMemberCreateDto Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int apartmentId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return RedirectToPage("/Account/Login");

        var apt = await _userService.GetApartmentWithDetailsAsync(apartmentId, userId.Value);
        if (apt == null)
        {
            TempData["ErrorMessage"] = "Bạn không có quyền gắn thành viên vào căn hộ này.";
            return RedirectToPage("./Index");
        }

        ApartmentId = apartmentId;
        ApartmentNumber = apt.ApartmentNumber;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int apartmentId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return RedirectToPage("/Account/Login");

        var apt = await _userService.GetApartmentWithDetailsAsync(apartmentId, userId.Value);
        if (apt == null)
        {
            TempData["ErrorMessage"] = "Bạn không có quyền gắn thành viên vào căn hộ này.";
            return RedirectToPage("./Index");
        }

        ApartmentId = apartmentId;
        ApartmentNumber = apt.ApartmentNumber;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var (success, errorField, errorMsg) = await _userService.CreateFamilyMemberAsync(
            Input, userId.Value, apartmentId);

        if (!success)
        {
            ModelState.AddModelError(errorField ?? string.Empty, errorMsg ?? "Đã xảy ra lỗi.");
            return Page();
        }

        TempData["SuccessMessage"] = string.IsNullOrWhiteSpace(Input.Email)
            ? $"Tài khoản thành viên gia đình đã được tạo thành công."
            : "Tài khoản thành viên gia đình đã được tạo. Thông tin đăng nhập đã được gửi đến email của họ.";

        return RedirectToPage("./Details", new { apartmentId });
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }
}
