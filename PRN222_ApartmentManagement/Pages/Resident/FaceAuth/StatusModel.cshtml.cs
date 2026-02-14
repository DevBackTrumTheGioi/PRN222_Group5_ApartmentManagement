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

    public Models.Resident Resident { get; set; } = null!;
    public List<FaceAuthHistory> RecentHistories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var resident = await _context.Residents.FindAsync(userId);
        if (resident == null)
        {
            return NotFound();
        }

        Resident = resident;
        RecentHistories = await _context.FaceAuthHistories
            .Where(h => h.ResidentId == userId)
            .OrderByDescending(h => h.AuthTime)
            .Take(3)
            .ToListAsync();

        return Page();
    }
}

