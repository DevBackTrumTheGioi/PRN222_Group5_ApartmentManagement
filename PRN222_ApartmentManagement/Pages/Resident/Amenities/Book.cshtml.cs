using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Amenities;

[Authorize(Roles = "Resident")]
public class BookModel : PageModel
{
    private readonly IAmenityService _amenityService;
    private readonly IResidentApartmentAccessService _residentApartmentAccessService;

    public BookModel(
        IAmenityService amenityService,
        IResidentApartmentAccessService residentApartmentAccessService)
    {
        _amenityService = amenityService;
        _residentApartmentAccessService = residentApartmentAccessService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Amenity? Amenity { get; set; }
    public IReadOnlyList<AmenityAvailabilitySlotDto> Slots { get; set; } = [];
    public List<SelectListItem> TimeOptions { get; set; } = [];
    public SelectList ApartmentOptions { get; set; } = null!;
    public bool HasApartment { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Vui lòng chọn căn hộ áp dụng.")]
        [Display(Name = "Căn hộ áp dụng")]
        public int? ApartmentId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sử dụng.")]
        [Display(Name = "Ngày sử dụng")]
        public DateTime BookingDate { get; set; } = DateTime.Now.Date;

        [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu.")]
        [Display(Name = "Giờ bắt đầu")]
        public string StartTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc.")]
        [Display(Name = "Giờ kết thúc")]
        public string EndTime { get; set; } = string.Empty;

        [Range(1, 500, ErrorMessage = "Số người tham gia phải lớn hơn 0.")]
        [Display(Name = "Số người tham gia")]
        public int ParticipantCount { get; set; } = 1;

        [MaxLength(500)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id, DateTime? date)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        Input.BookingDate = date?.Date ?? DateTime.Now.Date;
        await LoadApartmentOptionsAsync(userId.Value);
        HasApartment = ApartmentOptions.Any();

        var loaded = await LoadPageDataAsync(id);
        if (!loaded)
        {
            return NotFound();
        }

        if (!Amenity!.RequiresBooking)
        {
            TempData["ErrorMessage"] = "Tiện ích này đang mở tự do, không cần đặt trước.";
            return RedirectToPage("Index", new { selectedAmenityId = id, bookingDate = Input.BookingDate.ToString("yyyy-MM-dd") });
        }

        if (string.IsNullOrWhiteSpace(Input.StartTime) && Slots.Any(s => s.IsAvailable))
        {
            var firstSlot = Slots.First(s => s.IsAvailable);
            Input.StartTime = firstSlot.StartTime.ToString(@"hh\:mm");
            Input.EndTime = firstSlot.EndTime.ToString(@"hh\:mm");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        await LoadApartmentOptionsAsync(userId.Value);
        HasApartment = ApartmentOptions.Any();

        if (!TimeSpan.TryParse(Input.StartTime, out var startTime))
        {
            ModelState.AddModelError(nameof(Input.StartTime), "Giờ bắt đầu không hợp lệ.");
        }

        if (!TimeSpan.TryParse(Input.EndTime, out var endTime))
        {
            ModelState.AddModelError(nameof(Input.EndTime), "Giờ kết thúc không hợp lệ.");
        }

        var loaded = await LoadPageDataAsync(id);
        if (!loaded)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _amenityService.CreateBookingAsync(
            userId.Value,
            Input.ApartmentId!.Value,
            id,
            Input.BookingDate,
            startTime,
            endTime,
            Input.ParticipantCount,
            Input.Notes);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }

        TempData["SuccessMessage"] = $"{Amenity!.AmenityName}: {Input.BookingDate:dd/MM/yyyy} {startTime:hh\\:mm}-{endTime:hh\\:mm}";
        return RedirectToPage("MyBookings");
    }

    private async Task LoadApartmentOptionsAsync(int userId)
    {
        var apartments = await _residentApartmentAccessService.GetActiveApartmentOptionsAsync(userId);
        if (!Input.ApartmentId.HasValue && apartments.Count == 1)
        {
            Input.ApartmentId = apartments[0].ApartmentId;
        }

        ApartmentOptions = new SelectList(
            apartments.Select(a => new
            {
                Value = a.ApartmentId,
                Text = a.Display
            }),
            "Value",
            "Text",
            Input.ApartmentId);
    }

    private async Task<bool> LoadPageDataAsync(int amenityId)
    {
        Amenity = await _amenityService.GetAmenityByIdAsync(amenityId);
        if (Amenity == null)
        {
            return false;
        }

        Slots = await _amenityService.GetAvailabilitySlotsAsync(amenityId, Input.BookingDate);
        TimeOptions = BuildTimeOptions(Amenity);
        return true;
    }

    private static List<SelectListItem> BuildTimeOptions(Amenity amenity)
    {
        var items = new List<SelectListItem>();
        for (var time = amenity.OpenTime; time <= amenity.CloseTime; time += TimeSpan.FromHours(1))
        {
            var text = time.ToString(@"hh\:mm");
            items.Add(new SelectListItem(text, text));
        }

        return items;
    }

    private int? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : null;
    }
}
