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

    public List<object> TableRows { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? RoleFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

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

        TotalItems = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

        if (PageIndex < 1) PageIndex = 1;
        if (PageIndex > TotalPages && TotalPages > 0) PageIndex = TotalPages;

        Users = await query.OrderByDescending(u => u.CreatedAt)
                           .Skip((PageIndex - 1) * PageSize)
                           .Take(PageSize)
                           .ToListAsync();

        TableRows = Users.Select(u => (object)new {
            PrimaryId = u.UserId,
            User = new {
                Name = u.FullName,
                Subtitle = "@@" + u.Username,
                Avatar = string.Empty // Can add logic for initials-based avatar if needed
            },
            Contact = u.Email + "\n" + u.PhoneNumber,
            Role = u.Role?.ToString(),
            IsActive = u.IsActive,
            StatusText = u.IsActive ? "Hoạt động" : "Khóa",
            RoleBadgeClass = u.Role switch {
                UserRole.Admin => "bg-red-100 text-red-800",
                UserRole.Resident => "bg-blue-100 text-blue-800",
                _ => "bg-emerald-100 text-emerald-800"
            }
        }).ToList();
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
