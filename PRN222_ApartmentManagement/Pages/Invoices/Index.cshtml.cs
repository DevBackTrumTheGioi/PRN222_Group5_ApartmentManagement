using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using System.Security.Claims;

namespace PRN222_ApartmentManagement.Pages.Invoices;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public IndexModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<Invoice> Invoices { get; set; } = new List<Invoice>();

    public async Task OnGetAsync()
    {
        // If current user is a resident, only show invoices for their apartment
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (string.Equals(role, "Resident", StringComparison.OrdinalIgnoreCase))
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
                if (user != null && user.ApartmentId.HasValue)
                {
                    Invoices = await _context.Invoices
                        .Where(i => i.ApartmentId == user.ApartmentId.Value)
                        .Include(i => i.Apartment)
                        .Include(i => i.Creator)
                        .OrderByDescending(i => i.IssueDate)
                        .ToListAsync();
                    return;
                }
            }

            // if no apartment found, return empty list
            Invoices = new List<Invoice>();
            return;
        }

        // non-resident roles see all invoices
        Invoices = await _context.Invoices
            .Include(i => i.Apartment)
            .Include(i => i.Creator)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }
}
