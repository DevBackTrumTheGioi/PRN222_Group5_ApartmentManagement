using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.FaceAuth;

[Authorize(Roles = "BQL_Staff")]
public class AmenityAccessModel : PageModel
{
    private readonly IFaceAuthService _faceAuthService;
    private readonly IAmenityService _amenityService;

    public AmenityAccessModel(IFaceAuthService faceAuthService, IAmenityService amenityService)
    {
        _faceAuthService = faceAuthService;
        _amenityService = amenityService;
    }

    [BindProperty(SupportsGet = true)]
    public int? AmenityId { get; set; }

    [BindProperty]
    public ScanInputModel ScanInput { get; set; } = new();

    [BindProperty]
    public ManualInputModel ManualInput { get; set; } = new();

    public List<SelectListItem> AmenityOptions { get; set; } = new();
    public List<SelectListItem> ResidentOptions { get; set; } = new();
    public Amenity? SelectedAmenity { get; set; }
    public AmenityFaceAccessResultDto? Result { get; set; }

    public class ScanInputModel
    {
        public int? AmenityId { get; set; }
        public string? FaceDescriptorString { get; set; }
        public string? DeviceInfo { get; set; }
    }

    public class ManualInputModel
    {
        public int? AmenityId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn cư dân để check-in thủ công.")]
        [Display(Name = "Cư dân")]
        public int? ResidentId { get; set; }

        public string? DeviceInfo { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadPageDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostScanAsync()
    {
        AmenityId ??= ScanInput.AmenityId;
        await LoadPageDataAsync();

        if (!AmenityId.HasValue || SelectedAmenity == null)
        {
            ModelState.AddModelError(string.Empty, "Vui lòng chọn tiện ích cần kiểm tra trước khi quét.");
            return Page();
        }

        if (string.IsNullOrWhiteSpace(ScanInput.FaceDescriptorString))
        {
            ModelState.AddModelError(string.Empty, "Chưa thu được dữ liệu khuôn mặt từ camera.");
            return Page();
        }

        Result = await _faceAuthService.ValidateAmenityAccessAsync(
            AmenityId.Value,
            ScanInput.FaceDescriptorString,
            GetRemoteIpAddress(),
            ScanInput.DeviceInfo);

        return Page();
    }

    public async Task<IActionResult> OnPostManualAsync()
    {
        AmenityId ??= ManualInput.AmenityId;
        await LoadPageDataAsync();

        if (!AmenityId.HasValue || SelectedAmenity == null)
        {
            ModelState.AddModelError(string.Empty, "Vui lòng chọn tiện ích trước khi check-in thủ công.");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var staffUserId = GetCurrentUserId();
        if (staffUserId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        Result = await _faceAuthService.ValidateAmenityAccessManualAsync(
            AmenityId.Value,
            ManualInput.ResidentId!.Value,
            staffUserId.Value,
            GetRemoteIpAddress(),
            ManualInput.DeviceInfo);

        return Page();
    }

    private async Task LoadPageDataAsync()
    {
        var amenities = await _amenityService.GetResidentAmenitiesAsync();
        AmenityOptions = amenities
            .Select(a => new SelectListItem(
                $"{a.AmenityName} {(a.RequiresBooking ? "(Cần đặt chỗ)" : "(Mở tự do)")}",
                a.AmenityId.ToString(),
                AmenityId == a.AmenityId))
            .ToList();

        SelectedAmenity = AmenityId.HasValue
            ? amenities.FirstOrDefault(a => a.AmenityId == AmenityId.Value)
            : null;

        ScanInput.AmenityId ??= AmenityId;
        ManualInput.AmenityId ??= AmenityId;

        var residents = await _faceAuthService.GetResidentSummariesAsync(null, null);
        ResidentOptions = residents
            .Where(r => r.IsActive)
            .OrderBy(r => r.FullName)
            .Select(r => new SelectListItem(
                $"{r.FullName} - Căn {(r.ApartmentNumber ?? "-")}",
                r.UserId.ToString(),
                ManualInput.ResidentId == r.UserId))
            .ToList();
    }

    private int? GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdString, out var userId) ? userId : null;
    }

    private string? GetRemoteIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
