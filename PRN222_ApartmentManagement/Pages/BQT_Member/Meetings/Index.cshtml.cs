using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Pages.BQT_Member.Meetings;

[Authorize(Roles = "BQT_Member")]
public class IndexModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public IndexModel(ApartmentDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public MeetingType? TypeFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public MeetingStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    public IReadOnlyList<Meeting> Meetings { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var meetings = await _context.Meetings
            .AsNoTracking()
            .Include(m => m.Creator)
            .OrderByDescending(m => m.ScheduledDate)
            .ThenByDescending(m => m.CreatedAt)
            .ToListAsync();

        IEnumerable<Meeting> filtered = meetings;

        if (TypeFilter.HasValue)
            filtered = filtered.Where(m => m.MeetingType == TypeFilter.Value);

        if (StatusFilter.HasValue)
            filtered = filtered.Where(m => m.Status == StatusFilter.Value);

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var keyword = SearchQuery.Trim().ToLowerInvariant();
            filtered = filtered.Where(m =>
                m.Title.ToLower().Contains(keyword) ||
                (m.Location?.ToLower().Contains(keyword) ?? false) ||
                (m.Creator?.FullName?.ToLower().Contains(keyword) ?? false));
        }

        Meetings = filtered.ToList();
        return Page();
    }
}
