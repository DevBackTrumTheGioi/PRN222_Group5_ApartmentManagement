using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Resident.Amenities;

[Authorize(Policy = "ResidentOnly")]
public class MyBookingsModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public MyBookingsModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<AmenityBooking> Bookings { get; set; } = new();

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }

    public async Task OnGetAsync(int page = 1)
    {
        Page = page <= 0 ? 1 : page;

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return;

        var q = _context.AmenityBookings
            .Include(b => b.Amenity)
            .Where(b => b.ResidentId == user.UserId)
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.StartTime);

        TotalCount = await q.CountAsync();

        Bookings = await q.Skip((Page - 1) * PageSize).Take(PageSize).ToListAsync();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var booking = await _context.AmenityBookings.FindAsync(id);
        if (booking == null) return NotFound();

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Forbid();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || booking.ResidentId != user.UserId) return Forbid();

        // Only allow cancel for Pending or Confirmed bookings
        if (booking.Status != "Pending" && booking.Status != "Confirmed")
        {
            TempData["Error"] = "Chỉ có thể hủy booking đang chờ hoặc đã xác nhận.";
            return RedirectToPage();
        }

        // cancellation rule: allow cancel if now is at least 2 hours before start
        var bookingStart = booking.BookingDate.Date.Add(booking.StartTime);
        if (DateTime.Now.AddHours(2) > bookingStart)
        {
            TempData["Error"] = "Không thể hủy gần thời gian bắt đầu (ít hơn 2 giờ).";
            return RedirectToPage();
        }

        booking.Status = "Cancelled";
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã hủy booking.";
        return RedirectToPage();
    }
}
