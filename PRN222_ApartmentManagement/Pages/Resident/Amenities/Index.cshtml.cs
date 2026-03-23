using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Resident.Amenities;

[Authorize(Policy = "ResidentOnly")]
public class IndexModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public IndexModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<Amenity> Amenities { get; set; } = new();

    public List<AmenityType> AmenityTypes { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? AmenityFilter { get; set; }

    public async Task OnGetAsync()
    {
        Amenities = await _context.Amenities
            .Include(a => a.AmenityType)
            .Where(a => a.IsActive && !a.IsDeleted)
            .OrderBy(a => a.AmenityName)
            .ToListAsync();

        AmenityTypes = await _context.AmenityTypes
            .Where(t => t.IsActive && !t.IsDeleted)
            .OrderBy(t => t.TypeName)
            .ToListAsync();
    }

    // JSON endpoint for calendar events (kept for compatibility but resident index no longer shows calendar)
    public async Task<IActionResult> OnGetEventsAsync(int? amenityId, string? start, string? end)
    {
        if (!amenityId.HasValue) amenityId = AmenityFilter;

        DateTime? startDt = null, endDt = null;
        if (!string.IsNullOrEmpty(start))
        {
            if (DateTime.TryParse(start, out var s)) startDt = s;
        }
        if (!string.IsNullOrEmpty(end))
        {
            if (DateTime.TryParse(end, out var e)) endDt = e;
        }

        var q = _context.AmenityBookings.Include(b => b.Amenity).AsQueryable();
        if (amenityId.HasValue) q = q.Where(b => b.AmenityId == amenityId.Value);
        if (startDt.HasValue && endDt.HasValue)
        {
            q = q.Where(b => b.BookingDate >= startDt.Value.Date && b.BookingDate <= endDt.Value.Date);
        }

        var bookings = await q.Where(b => b.Status != "Cancelled").ToListAsync();

        var events = bookings.Select(b => new
        {
            id = b.BookingId,
            title = $"{b.Amenity.AmenityName} - {b.Status}",
            start = b.BookingDate.Add(b.StartTime).ToString("o"),
            end = b.BookingDate.Add(b.EndTime).ToString("o"),
            extendedProps = new { amount = b.TotalAmount, notes = b.Notes, amenityId = b.AmenityId }
        });

        return new JsonResult(events);
    }
}
