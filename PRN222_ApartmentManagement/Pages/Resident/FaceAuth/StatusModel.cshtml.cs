using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Resident.FaceAuth;

[Authorize(Roles = "Resident")]
public class StatusModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public StatusModel(ApartmentDbContext context)
    {
        _context = context;
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

        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.Role != UserRole.Resident)
        {
            return NotFound();
        }

        Resident = user;
        RecentHistories = await _context.FaceAuthHistories
            .Where(h => h.ResidentId == userId)
            .OrderByDescending(h => h.AuthTime)
            .Take(3)
            .ToListAsync();

        return Page();
    }
}
