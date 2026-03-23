using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Announcements;

[Authorize(Roles = "BQL_Manager,BQT_Head")]
public class IndexModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public IndexModel(ApartmentDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActiveFilter { get; set; }

    public List<Announcement> Announcements { get; set; } = [];

    public string AnnouncementActionLabel { get; private set; } = string.Empty;
    public string SourceCode { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!TryResolveSource(out var source, out _))
        {
            return Forbid();
        }

        var query = _context.Announcements
            .Include(a => a.Creator)
            .Where(a => !a.IsDeleted && a.Source == source)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var keyword = Search.Trim().ToLower();
            query = query.Where(a => a.Title.ToLower().Contains(keyword) || a.Content.ToLower().Contains(keyword));
        }

        if (IsActiveFilter.HasValue)
        {
            query = query.Where(a => a.IsActive == IsActiveFilter.Value);
        }

        Announcements = await query
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.PublishedDate)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (!TryResolveSource(out var source, out _))
        {
            return Forbid();
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var announcement = await _context.Announcements
            .FirstOrDefaultAsync(a => a.AnnouncementId == id && !a.IsDeleted && a.Source == source);

        if (announcement == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy thông báo để xóa.";
            return RedirectToPage(new { Search, IsActiveFilter });
        }

        announcement.IsDeleted = true;
        announcement.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã xóa thông báo (soft delete).";
        return RedirectToPage(new { Search, IsActiveFilter });
    }

    private bool TryResolveSource(out string source, out string actionLabel)
    {
        if (User.IsInRole("BQL_Manager"))
        {
            source = "BQL";
            actionLabel = "Đăng thông báo vận hành";
            SourceCode = source;
            AnnouncementActionLabel = actionLabel;
            return true;
        }

        if (User.IsInRole("BQT_Head"))
        {
            source = "BQT";
            actionLabel = "Đăng thông báo chính sách";
            SourceCode = source;
            AnnouncementActionLabel = actionLabel;
            return true;
        }

        source = string.Empty;
        actionLabel = string.Empty;
        return false;
    }

    private int? GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
