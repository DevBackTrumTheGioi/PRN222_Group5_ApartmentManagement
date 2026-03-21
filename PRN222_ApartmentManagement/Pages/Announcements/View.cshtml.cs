using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Pages.Announcements;

[Authorize(Roles = "Resident")]
public class ViewModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public ViewModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<AnnouncementItemViewModel> Announcements { get; set; } = [];

    public class AnnouncementItemViewModel
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public AnnouncementPriority Priority { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        await LoadAnnouncementsAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkAsReadAsync(int announcementId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        if (announcementId <= 0)
        {
            TempData["ErrorMessage"] = "Thông báo không hợp lệ.";
            return RedirectToPage();
        }

        var now = DateTime.Now;
        var announcementExists = await _context.Announcements.AnyAsync(a =>
            a.AnnouncementId == announcementId &&
            !a.IsDeleted &&
            a.IsActive &&
            a.PublishedDate <= now &&
            (!a.ExpiryDate.HasValue || a.ExpiryDate >= now));

        if (!announcementExists)
        {
            TempData["ErrorMessage"] = "Thông báo không tồn tại hoặc đã hết hiệu lực.";
            return RedirectToPage();
        }

        var readState = await _context.AnnouncementReads.FirstOrDefaultAsync(ar =>
            ar.UserId == userId.Value && ar.AnnouncementId == announcementId);

        if (readState == null)
        {
            _context.AnnouncementReads.Add(new AnnouncementRead
            {
                UserId = userId.Value,
                AnnouncementId = announcementId,
                ReadAt = now
            });
        }
        else
        {
            readState.ReadAt = now;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã đánh dấu thông báo là đã đọc.";

        return RedirectToPage();
    }

    private async Task LoadAnnouncementsAsync(int userId)
    {
        var now = DateTime.Now;

        var readMap = await _context.AnnouncementReads
            .Where(ar => ar.UserId == userId)
            .ToDictionaryAsync(ar => ar.AnnouncementId, ar => ar.ReadAt);

        var announcements = await _context.Announcements
            .Where(a => !a.IsDeleted
                        && a.IsActive
                        && a.PublishedDate <= now
                        && (!a.ExpiryDate.HasValue || a.ExpiryDate >= now))
            .OrderByDescending(a => a.Priority)
            .ThenByDescending(a => a.PublishedDate)
            .Select(a => new AnnouncementItemViewModel
            {
                AnnouncementId = a.AnnouncementId,
                Title = a.Title,
                Content = a.Content,
                Source = a.Source,
                PublishedDate = a.PublishedDate,
                ExpiryDate = a.ExpiryDate,
                Priority = a.Priority
            })
            .ToListAsync();

        foreach (var item in announcements)
        {
            if (readMap.TryGetValue(item.AnnouncementId, out var readAt))
            {
                item.IsRead = true;
                item.ReadAt = readAt;
            }
        }

        Announcements = announcements;
    }

    private int? GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
