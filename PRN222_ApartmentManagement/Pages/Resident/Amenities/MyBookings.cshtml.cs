using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Resident.Amenities;

[Authorize(Roles = "Resident")]
public class MyBookingsModel : PageModel
{
    private readonly IAmenityService _amenityService;

    public MyBookingsModel(IAmenityService amenityService)
    {
        _amenityService = amenityService;
    }

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    public IReadOnlyList<AmenityBooking> Bookings { get; set; } = [];
    public IReadOnlyList<AmenityBooking> ActiveBookings { get; set; } = [];

    public int TotalCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public int ActiveWindowCount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var allBookings = await _amenityService.GetResidentBookingsAsync(userId.Value);
        TotalCount = allBookings.Count;
        ConfirmedCount = allBookings.Count(b => b.Status == AmenityBookingStatusHelper.Confirmed);
        CompletedCount = allBookings.Count(b => b.Status == AmenityBookingStatusHelper.Completed);
        CancelledCount = allBookings.Count(b => b.Status == AmenityBookingStatusHelper.Cancelled);
        ActiveBookings = allBookings.Where(IsActiveWindow).ToList();
        ActiveWindowCount = ActiveBookings.Count;

        Bookings = string.IsNullOrWhiteSpace(StatusFilter)
            ? allBookings
            : allBookings.Where(b => b.Status == StatusFilter).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var result = await _amenityService.CancelBookingAsync(bookingId, userId.Value);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

        return RedirectToPage(new { StatusFilter });
    }

    private int? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : null;
    }

    private static bool IsActiveWindow(AmenityBooking booking)
    {
        if (booking.Status == AmenityBookingStatusHelper.Cancelled)
        {
            return false;
        }

        var now = DateTime.Now;
        return booking.BookingDate.Date > now.Date ||
               (booking.BookingDate.Date == now.Date && booking.EndTime > now.TimeOfDay);
    }
}
