using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Amenities;

[Authorize(Policy = "AdminAndBQLManager")]
public class EditModel : PageModel
{
    private readonly IAmenityService _amenityService;

    public EditModel(IAmenityService amenityService)
    {
        _amenityService = amenityService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<AmenityType> AmenityTypes { get; set; } = [];

    public class InputModel
    {
        public int AmenityId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tiện ích.")]
        [MaxLength(100)]
        [Display(Name = "Tên tiện ích")]
        public string AmenityName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại tiện ích.")]
        [Display(Name = "Loại tiện ích")]
        public int? AmenityTypeId { get; set; }

        [MaxLength(200)]
        [Display(Name = "Vị trí")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập sức chứa.")]
        [Range(1, 500, ErrorMessage = "Sức chứa phải từ 1 đến 500.")]
        [Display(Name = "Sức chứa")]
        public int? Capacity { get; set; }

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Giá theo giờ không hợp lệ.")]
        [Display(Name = "Giá theo giờ")]
        public decimal? PricePerHour { get; set; }

        public bool RequiresBooking { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giờ mở cửa.")]
        [Display(Name = "Giờ mở cửa")]
        public TimeSpan OpenTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giờ đóng cửa.")]
        [Display(Name = "Giờ đóng cửa")]
        public TimeSpan CloseTime { get; set; }

        [Range(0, 72, ErrorMessage = "Thời hạn hủy phải từ 0 đến 72 giờ.")]
        [Display(Name = "Hạn hủy trước giờ dùng")]
        public int CancellationDeadlineHours { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var amenity = await _amenityService.GetAmenityByIdAsync(id);
        if (amenity == null)
        {
            return NotFound();
        }

        AmenityTypes = await _amenityService.GetAmenityTypesAsync();
        Input = new InputModel
        {
            AmenityId = amenity.AmenityId,
            AmenityName = amenity.AmenityName,
            AmenityTypeId = amenity.AmenityTypeId,
            Location = amenity.Location,
            Capacity = amenity.Capacity,
            PricePerHour = amenity.PricePerHour,
            RequiresBooking = amenity.RequiresBooking,
            OpenTime = amenity.OpenTime,
            CloseTime = amenity.CloseTime,
            CancellationDeadlineHours = amenity.CancellationDeadlineHours,
            IsActive = amenity.IsActive,
            Description = amenity.Description
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        AmenityTypes = await _amenityService.GetAmenityTypesAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var amenity = new Amenity
        {
            AmenityId = Input.AmenityId,
            AmenityName = Input.AmenityName,
            AmenityTypeId = Input.AmenityTypeId,
            Location = Input.Location,
            Capacity = Input.Capacity,
            PricePerHour = Input.PricePerHour ?? 0,
            RequiresBooking = Input.RequiresBooking,
            OpenTime = Input.OpenTime,
            CloseTime = Input.CloseTime,
            CancellationDeadlineHours = Input.CancellationDeadlineHours,
            IsActive = Input.IsActive,
            Description = Input.Description
        };

        var result = await _amenityService.SaveAmenityAsync(amenity);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToPage("Index");
    }
}
