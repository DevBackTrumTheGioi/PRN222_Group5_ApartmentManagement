using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.FaceAuth;

[Authorize(Roles = "Resident")]
public class RegisterModel : PageModel
{
    private readonly IFaceAuthService _faceAuthService;

    public RegisterModel(IFaceAuthService faceAuthService)
    {
        _faceAuthService = faceAuthService;
    }

    [BindProperty]
    public string? FaceDescriptorString { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(FaceDescriptorString))
        {
            return Page();
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var (success, errorMessage) = await _faceAuthService.RegisterFaceAsync(userId, FaceDescriptorString);
        if (!success)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            return Page();
        }

        return RedirectToPage("./Status");
    }
}
