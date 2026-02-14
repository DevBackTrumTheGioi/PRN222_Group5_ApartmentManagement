using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Resident.FaceAuth;

[Authorize(Roles = "Resident")]
public class HistoryModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public HistoryModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<FaceAuthHistory> Histories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToPage("/Account/Login");
        }

        Histories = await _context.FaceAuthHistories
            .Where(h => h.ResidentId == userId)
            .OrderByDescending(h => h.AuthTime)
            .ToListAsync();

        return Page();
    }
}

