using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Resident.Amenities;

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

    public class InputModel
    {
        [Required]
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

    public async Task OnGetAsync()
    {
        Amenities = await _context.Amenities.Where(a => a.IsActive && !a.IsDeleted).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Amenities = await _context.Amenities.Where(a => a.IsActive && !a.IsDeleted).ToListAsync();

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

        return RedirectToPage("MyBookings");
    }
}
