using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.FaceAuth;

[Authorize(Roles = "BQL_Staff")]
public class RegisterModel : PageModel
{
    private readonly IFaceAuthService _faceAuthService;

    public RegisterModel(IFaceAuthService faceAuthService)
    {
        _faceAuthService = faceAuthService;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public string? FaceDescriptorString { get; set; }

    public User Resident { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        return await LoadResidentAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(FaceDescriptorString))
        {
            ModelState.AddModelError(string.Empty, "Dữ liệu khuôn mặt không hợp lệ.");
            return await LoadResidentAsync();
        }

        var actorUserId = GetCurrentUserId();
        if (actorUserId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var result = await _faceAuthService.RegisterFaceForResidentAsync(actorUserId.Value, Id, FaceDescriptorString);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể lưu dữ liệu Face ID.");
            return await LoadResidentAsync();
        }

        TempData["SuccessMessage"] = "Đã hỗ trợ đăng ký Face ID thành công.";
        return RedirectToPage("./Index");
    }

    private async Task<IActionResult> LoadResidentAsync()
    {
        var resident = await _faceAuthService.GetResidentForManagementAsync(Id);
        if (resident == null)
        {
            return NotFound();
        }

        Resident = resident;
        return Page();
    }

    private int? GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdString, out var userId) ? userId : null;
    }
}
