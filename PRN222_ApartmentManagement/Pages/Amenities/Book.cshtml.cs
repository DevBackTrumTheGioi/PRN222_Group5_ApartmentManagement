using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Amenities;

[Authorize(Policy = "ResidentOnly")]
public class BookModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public BookModel(ApartmentDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<Amenity> Amenities { get; set; } = new();

    public IEnumerable<SelectListItem> AmenitySelectList { get; set; } = Enumerable.Empty<SelectListItem>();

    public class InputModel
    {
        [Required(ErrorMessage = "Vui lòng chọn tiện ích.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn tiện ích.")]
        public int AmenityId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public string? Notes { get; set; }
    }

    private async Task PopulateAmenitySelectAsync()
    {
        Amenities = await _context.Amenities.Where(a => a.IsActive && !a.IsDeleted).ToListAsync();
        AmenitySelectList = Amenities.Select(a => new SelectListItem(a.AmenityName, a.AmenityId.ToString()));
    }

    public async Task OnGetAsync(int? amenityId, string? date, string? start, string? end)
    {
        await PopulateAmenitySelectAsync();

        if (amenityId.HasValue) Input.AmenityId = amenityId.Value;
        if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var d)) Input.BookingDate = d.Date;

        // parse time in HH:mm or HH:mm:ss or ISO
        if (!string.IsNullOrEmpty(start))
        {
            if (TimeSpan.TryParse(start, out var st)) Input.StartTime = st;
            else if (DateTime.TryParse(start, out var dst)) Input.StartTime = dst.TimeOfDay;
        }
        if (!string.IsNullOrEmpty(end))
        {
            if (TimeSpan.TryParse(end, out var en)) Input.EndTime = en;
            else if (DateTime.TryParse(end, out var den)) Input.EndTime = den.TimeOfDay;
        }

        // If start/end missing but booking date provided, set reasonable defaults
        if ((string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end)) && !string.IsNullOrEmpty(date))
        {
            if (Input.StartTime == default) Input.StartTime = new TimeSpan(9, 0, 0);
            if (Input.EndTime == default) Input.EndTime = new TimeSpan(10, 0, 0);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateAmenitySelectAsync();

        if (!ModelState.IsValid)
            return Page();

        if (Input.EndTime <= Input.StartTime)
        {
            ModelState.AddModelError(string.Empty, "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
            return Page();
        }

        var durationHours = (Input.EndTime - Input.StartTime).TotalHours;
        if (durationHours <= 0 || durationHours > 12)
        {
            ModelState.AddModelError(string.Empty, "Thời lượng đặt phải lớn hơn 0 và không vượt quá 12 giờ.");
            return Page();
        }

        var amenity = await _context.Amenities.FindAsync(Input.AmenityId);
        if (amenity == null || amenity.IsDeleted || !amenity.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Tiện ích không tồn tại hoặc không khả dụng.");
            return Page();
        }

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Forbid();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return Forbid();

        if (!user.ApartmentId.HasValue || user.ApartmentId.Value == 0)
        {
            ModelState.AddModelError(string.Empty, "Bạn cần liên kết căn hộ để có thể đặt tiện ích. Vui lòng cập nhật thông tin căn hộ.");
            return Page();
        }

        var bookingStart = Input.BookingDate.Date.Add(Input.StartTime);
        if (bookingStart < DateTime.Now.AddMinutes(-1))
        {
            ModelState.AddModelError(string.Empty, "Không thể đặt thời gian trong quá khứ.");
            return Page();
        }

        // Check overlap
        var overlap = await _context.AmenityBookings.AnyAsync(b =>
            b.AmenityId == Input.AmenityId &&
            b.BookingDate == Input.BookingDate.Date &&
            b.Status != "Cancelled" &&
            !(b.EndTime <= Input.StartTime || b.StartTime >= Input.EndTime));

        if (overlap)
        {
            ModelState.AddModelError(string.Empty, "Khoảng thời gian này đã bị đặt. Vui lòng chọn khung giờ khác.");
            return Page();
        }

        var totalHours = (decimal)durationHours;
        var totalAmount = (amenity.PricePerHour ?? 0m) * totalHours;

        var booking = new AmenityBooking
        {
            AmenityId = Input.AmenityId,
            ApartmentId = user.ApartmentId.Value,
            ResidentId = user.UserId,
            BookingDate = Input.BookingDate.Date,
            StartTime = Input.StartTime,
            EndTime = Input.EndTime,
            TotalHours = totalHours,
            TotalAmount = totalAmount,
            Status = "Pending",
            Notes = Input.Notes,
            CreatedAt = DateTime.Now
        };

        _context.AmenityBookings.Add(booking);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đặt tiện ích thành công. Trạng thái: Pending.";

        // Redirect back to calendar index so the new booking appears and preserve amenity filter
        return RedirectToPage("Index", new { amenityFilter = booking.AmenityId, date = booking.BookingDate.ToString("yyyy-MM-dd") });
    }
}
