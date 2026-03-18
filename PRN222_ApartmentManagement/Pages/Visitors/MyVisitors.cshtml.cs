using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Visitors;

[Authorize(Policy = "ResidentOnly")]
public class MyVisitorsModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public MyVisitorsModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<Visitor> Visitors { get; set; } = new();

    public async Task OnGetAsync()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !user.ApartmentId.HasValue) return;

        Visitors = await _context.Visitors
            .Where(v => v.ApartmentId == user.ApartmentId.Value)
            .OrderByDescending(v => v.VisitDate)
            .ThenByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var visitor = await _context.Visitors.FindAsync(id);
        if (visitor == null) return NotFound();

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Forbid();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || user.UserId != visitor.RegisteredBy) return Forbid();

        if (visitor.Status != PRN222_ApartmentManagement.Models.Enums.VisitorStatus.Pending)
        {
            TempData["Error"] = "Chỉ có thể hủy khách khi trạng thái đang chờ.";
            return RedirectToPage();
        }

        visitor.Status = PRN222_ApartmentManagement.Models.Enums.VisitorStatus.Cancelled;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã hủy đăng ký khách.";
        return RedirectToPage();
    }
}
