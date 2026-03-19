using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Amenities;

[Authorize] // 🔥 Cho phép tất cả user đăng nhập (phân role bên trong)
public class IndexModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public IndexModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<Amenity> Amenities { get; set; } = new();

    // dùng cho resident filter
    [BindProperty(SupportsGet = true)]
    public int? AmenityFilter { get; set; }

    // check role
    public bool IsAdmin =>
        User.IsInRole("Admin") ||
        User.IsInRole("BQL_Manager") ||
        User.IsInRole("BQT_Head");

    // ===================== LOAD PAGE =====================
    public async Task OnGetAsync()
    {
        if (IsAdmin)
        {
            // ADMIN: lấy tất cả (để quản lý)
            Amenities = await _context.Amenities
                .Include(a => a.AmenityType)
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.AmenityName)
                .ToListAsync();
        }
        else
        {
            // RESIDENT: chỉ lấy cái active
            Amenities = await _context.Amenities
                .Where(a => a.IsActive && !a.IsDeleted)
                .ToListAsync();
        }
    }

    // ===================== ADMIN ACTION =====================

    public async Task<IActionResult> OnPostToggleActiveAsync(int id)
    {
        if (!IsAdmin) return Forbid(); // 🔥 chặn resident

        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return NotFound();

        amenity.IsActive = !amenity.IsActive;
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (!IsAdmin) return Forbid(); // 🔥 chặn resident

        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return NotFound();

        amenity.IsDeleted = true;
        amenity.IsActive = false;

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    // ===================== RESIDENT CALENDAR =====================

    public async Task<IActionResult> OnGetEventsAsync(int? amenityId, string? start, string? end)
    {
        // nếu không truyền thì lấy từ filter
        if (!amenityId.HasValue) amenityId = AmenityFilter;

        DateTime? startDt = null, endDt = null;

        if (!string.IsNullOrEmpty(start) && DateTime.TryParse(start, out var s))
            startDt = s;

        if (!string.IsNullOrEmpty(end) && DateTime.TryParse(end, out var e))
            endDt = e;

        var q = _context.AmenityBookings
            .Include(b => b.Amenity)
            .AsQueryable();

        if (amenityId.HasValue)
            q = q.Where(b => b.AmenityId == amenityId.Value);

        if (startDt.HasValue && endDt.HasValue)
        {
            q = q.Where(b =>
                b.BookingDate >= startDt.Value.Date &&
                b.BookingDate <= endDt.Value.Date);
        }

        var bookings = await q
            .Where(b => b.Status != "Cancelled")
            .ToListAsync();

        var events = bookings.Select(b => new
        {
            id = b.BookingId,
            title = $"{b.Amenity.AmenityName} - {b.Status}",
            start = b.BookingDate.Add(b.StartTime).ToString("o"),
            end = b.BookingDate.Add(b.EndTime).ToString("o"),
            extendedProps = new
            {
                amount = b.TotalAmount,
                notes = b.Notes,
                amenityId = b.AmenityId
            }
        });

        return new JsonResult(events);
    }
}