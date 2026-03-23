using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Announcements;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public DetailsModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public Announcement Announcement { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0) return NotFound();

        var announcement = await _context.Announcements
            .Include(a => a.Attachments)
            .Include(a => a.Creator)
            .FirstOrDefaultAsync(a => a.AnnouncementId == id && !a.IsDeleted);

        if (announcement == null) return NotFound();

        if (User.IsInRole("Resident"))
        {
            var now = DateTime.Now;
            if (!announcement.IsActive || announcement.PublishedDate > now || (announcement.ExpiryDate.HasValue && announcement.ExpiryDate < now))
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null) return Forbid();

            var read = await _context.AnnouncementReads
                .FirstOrDefaultAsync(ar => ar.AnnouncementId == id && ar.UserId == userId.Value);

            if (read == null)
            {
                _context.AnnouncementReads.Add(new AnnouncementRead
                {
                    AnnouncementId = id,
                    UserId = userId.Value,
                    ReadAt = now
                });
            }
            else
            {
                read.ReadAt = now;
            }

            await _context.SaveChangesAsync();
        }

        Announcement = announcement;
        return Page();
    }

    private int? GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
