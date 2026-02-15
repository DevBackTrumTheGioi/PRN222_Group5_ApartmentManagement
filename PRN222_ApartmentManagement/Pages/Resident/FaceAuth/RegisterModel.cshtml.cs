using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Resident.FaceAuth;

[Authorize(Roles = "Resident")]
public class RegisterModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public RegisterModel(ApartmentDbContext context)
    {
        _context = context;
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

        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.Role != UserRole.Resident)
        {
            return NotFound();
        }

        user.FaceDescriptor = FaceDescriptorString;
        user.IsFaceRegistered = true;
        user.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Status");
    }
}
