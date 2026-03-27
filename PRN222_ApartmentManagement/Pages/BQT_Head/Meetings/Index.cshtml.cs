using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.BQT_Head.Meetings;

[Authorize(Roles = "BQT_Head")]
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

    public int TotalCount { get; set; }
    public int ScheduledCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }

    public bool HasFilter => TypeFilter.HasValue || StatusFilter.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task<IActionResult> OnGetAsync()
    {
        var query = _context.Meetings
            .AsNoTracking()
            .Include(m => m.Creator)
            .OrderByDescending(m => m.ScheduledDate)
            .ThenByDescending(m => m.CreatedAt)
            .AsQueryable();

        var allMeetings = await query.ToListAsync();

        TotalCount = allMeetings.Count;
        ScheduledCount = allMeetings.Count(m => m.Status == MeetingStatus.Scheduled);
        InProgressCount = allMeetings.Count(m => m.Status == MeetingStatus.InProgress);
        CompletedCount = allMeetings.Count(m => m.Status == MeetingStatus.Completed);

        IEnumerable<Meeting> filtered = allMeetings;

        if (TypeFilter.HasValue)
        {
            filtered = filtered.Where(m => m.MeetingType == TypeFilter.Value);
        }

        if (StatusFilter.HasValue)
        {
            filtered = filtered.Where(m => m.Status == StatusFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var keyword = SearchQuery.Trim().ToLowerInvariant();
            filtered = filtered.Where(m =>
                m.Title.ToLower().Contains(keyword) ||
                (m.Location?.ToLower().Contains(keyword) ?? false) ||
                (m.Creator?.FullName?.ToLower().Contains(keyword) ?? false));
        }

        Meetings = filtered
            .OrderBy(m => m.Status == MeetingStatus.Completed ? 1 : 0)
            .ThenBy(m => m.ScheduledDate)
            .ThenBy(m => m.StartTime)
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var meeting = await _context.Meetings.FirstOrDefaultAsync(m => m.MeetingId == id);
        if (meeting == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy cuộc họp.";
            return RedirectToPage();
        }

        _context.Meetings.Remove(meeting);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã xóa cuộc họp.";
        return RedirectToPage(new { TypeFilter, StatusFilter, SearchQuery });
    }

    public static string GetMeetingTime(Meeting meeting)
    {
        var start = meeting.StartTime.HasValue ? meeting.StartTime.Value.ToString(@"hh\:mm") : "--:--";
        var end = meeting.EndTime.HasValue ? meeting.EndTime.Value.ToString(@"hh\:mm") : "--:--";
        return $"{start} - {end}";
    }
}
