using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.Amenities;

[Authorize(Roles = "BQL_Staff")]
public class BookModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public BookModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<AmenityBooking> Bookings { get; set; } = new();

    // nullable: null = no filter (show all)
    public DateTime? DateFilter { get; set; }

    // Paging
    public int Page { get; set; } = 1; // current page index
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public async Task OnGetAsync(DateTime? date, int pageIndex = 1, int pageSize = 10)
    {
        DateFilter = date?.Date;
        Page = pageIndex <= 0 ? 1 : pageIndex;
        PageSize = pageSize <= 0 ? 10 : pageSize;

        var q = _context.AmenityBookings
            .Include(b => b.Amenity)
            .Include(b => b.Resident).ThenInclude(r => r.Apartment)
            .AsQueryable();

        if (DateFilter.HasValue)
        {
            q = q.Where(b => b.BookingDate == DateFilter.Value);
        }

        TotalCount = await q.CountAsync();

        Bookings = await q
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostConfirmAsync(int id, DateTime? date, int pageIndex = 1, int pageSize = 10)
    {
        DateFilter = date?.Date;
        Page = pageIndex <= 0 ? 1 : pageIndex;
        PageSize = pageSize <= 0 ? 10 : pageSize;

        var booking = await _context.AmenityBookings.FindAsync(id);
        if (booking == null) return NotFound();

        booking.Status = "Confirmed";
        booking.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã xác nhận booking.";
        return RedirectToPage(new { date = DateFilter, pageIndex = Page, pageSize = PageSize });
    }

    public async Task<IActionResult> OnPostCheckInAsync(int id, DateTime? date, int pageIndex = 1, int pageSize = 10)
    {
        DateFilter = date?.Date;
        Page = pageIndex <= 0 ? 1 : pageIndex;
        PageSize = pageSize <= 0 ? 10 : pageSize;

        var booking = await _context.AmenityBookings.FindAsync(id);
        if (booking == null) return NotFound();

        booking.Status = "CheckedIn";
        booking.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã xác nhận check-in.";
        return RedirectToPage(new { date = DateFilter, pageIndex = Page, pageSize = PageSize });
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id, DateTime? date, int pageIndex = 1, int pageSize = 10)
    {
        DateFilter = date?.Date;
        Page = pageIndex <= 0 ? 1 : pageIndex;
        PageSize = pageSize <= 0 ? 10 : pageSize;

        var booking = await _context.AmenityBookings.FindAsync(id);
        if (booking == null) return NotFound();

        booking.Status = "Completed";
        booking.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã hoàn tất booking.";
        return RedirectToPage(new { date = DateFilter, pageIndex = Page, pageSize = PageSize });
    }

    public async Task<IActionResult> OnPostCancelAsync(int id, DateTime? date, int pageIndex = 1, int pageSize = 10)
    {
        DateFilter = date?.Date;
        Page = pageIndex <= 0 ? 1 : pageIndex;
        PageSize = pageSize <= 0 ? 10 : pageSize;

        var booking = await _context.AmenityBookings.FindAsync(id);
        if (booking == null) return NotFound();

        booking.Status = "Cancelled";
        booking.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã hủy booking.";
        return RedirectToPage(new { date = DateFilter, pageIndex = Page, pageSize = PageSize });
    }
}
