using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Admin.Users;

[Authorize(Policy = "AdminOnly")]
public class IndexModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public IndexModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<User> Users { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? RoleFilter { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrEmpty(SearchTerm))
        {
            query = query.Where(u => u.FullName.Contains(SearchTerm) || 
                                     u.Username.Contains(SearchTerm) || 
                                     (u.Email != null && u.Email.Contains(SearchTerm)));
        }

        if (!string.IsNullOrEmpty(RoleFilter) && Enum.TryParse<UserRole>(RoleFilter, out var role))
        {
            query = query.Where(u => u.Role == role);
        }

        Users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsDeleted = true;
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}

