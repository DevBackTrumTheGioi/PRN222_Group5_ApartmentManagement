using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Amenities;

[Authorize(Policy = "AdminAndBQLManager")]
public class BookingsModel : PageModel
{
    private readonly IAmenityService _amenityService;

    public BookingsModel(IAmenityService amenityService)
    {
        _amenityService = amenityService;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? AmenityId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool ActiveOnly { get; set; }

    public IReadOnlyList<AmenityBooking> Bookings { get; set; } = [];
    public List<SelectListItem> AmenityOptions { get; set; } = [];

    public int TotalCount { get; set; }
    public int ActiveWindowCount { get; set; }
    public int TodayCount { get; set; }
    public int CancelledCount { get; set; }

    public bool HasFilter =>
        !string.IsNullOrWhiteSpace(SearchTerm) ||
        !string.IsNullOrWhiteSpace(StatusFilter) ||
        AmenityId.HasValue ||
        FromDate.HasValue ||
        ToDate.HasValue ||
        ActiveOnly;

    public async Task OnGetAsync()
    {
        await LoadPageDataAsync();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int bookingId, string status)
    {
        var result = await _amenityService.UpdateBookingStatusAsync(bookingId, status);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

        return RedirectToPage(new
        {
            SearchTerm,
            StatusFilter,
            AmenityId,
            FromDate = FromDate?.ToString("yyyy-MM-dd"),
            ToDate = ToDate?.ToString("yyyy-MM-dd"),
            ActiveOnly
        });
    }

    public bool IsActiveWindow(AmenityBooking booking)
    {
        if (booking.Status == AmenityBookingStatusHelper.Cancelled)
        {
            return false;
        }

        var now = DateTime.Now;
        return booking.BookingDate.Date > now.Date ||
               (booking.BookingDate.Date == now.Date && booking.EndTime > now.TimeOfDay);
    }

    public bool CanCancel(AmenityBooking booking)
    {
        return booking.Status == AmenityBookingStatusHelper.Confirmed;
    }

    public bool CanComplete(AmenityBooking booking)
    {
        return booking.Status == AmenityBookingStatusHelper.Confirmed &&
               booking.BookingDate.Date <= DateTime.Now.Date;
    }

    private async Task LoadPageDataAsync()
    {
        var allAmenities = await _amenityService.GetManagerAmenitiesAsync(null, true, null);
        AmenityOptions = allAmenities
            .OrderBy(a => a.AmenityName)
            .Select(a => new SelectListItem(a.AmenityName, a.AmenityId.ToString(), AmenityId == a.AmenityId))
            .ToList();

        var bookings = await _amenityService.GetManagerBookingsAsync(SearchTerm, StatusFilter, AmenityId, FromDate, ToDate);

        TotalCount = bookings.Count;
        ActiveWindowCount = bookings.Count(IsActiveWindow);
        TodayCount = bookings.Count(b => b.BookingDate.Date == DateTime.Now.Date && b.Status != AmenityBookingStatusHelper.Cancelled);
        CancelledCount = bookings.Count(b => b.Status == AmenityBookingStatusHelper.Cancelled);

        Bookings = ActiveOnly
            ? bookings.Where(IsActiveWindow).ToList()
            : bookings;
    }
}
