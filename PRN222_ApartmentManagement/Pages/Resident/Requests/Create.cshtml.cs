using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Resident.Requests;

[Authorize(Roles = "Resident")]
public class CreateModel : PageModel
{
    private readonly IRequestService _requestService;
    private readonly IResidentApartmentAccessService _residentApartmentAccessService;

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg", "image/png", "image/gif", "image/webp",
        "application/pdf"
    ];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public CreateModel(
        IRequestService requestService,
        IResidentApartmentAccessService residentApartmentAccessService)
    {
        _requestService = requestService;
        _residentApartmentAccessService = residentApartmentAccessService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public List<IFormFile>? Attachments { get; set; }

    public SelectList RequestTypeOptions { get; set; } = null!;
    public SelectList ApartmentOptions { get; set; } = null!;

    public class InputModel
    {
        [Required(ErrorMessage = "Vui lòng chọn căn hộ áp dụng.")]
        [Display(Name = "Căn hộ áp dụng")]
        public int? ApartmentId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không vượt quá 200 ký tự.")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Loại yêu cầu")]
        public RequestType? RequestType { get; set; }

        [Display(Name = "Mô tả chi tiết")]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hasActiveApartment = await _residentApartmentAccessService.HasAnyActiveApartmentAsync(userId);

        if (!hasActiveApartment)
        {
            TempData["ErrorMessage"] = "Bạn chưa được gán căn hộ. Vui lòng liên hệ Ban Quản Lý.";
            return RedirectToPage("/Resident/Index");
        }

        await LoadSelectListsAsync(userId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hasActiveApartment = await _residentApartmentAccessService.HasAnyActiveApartmentAsync(userId);

        if (!hasActiveApartment)
        {
            TempData["ErrorMessage"] = "Bạn chưa được gán căn hộ. Vui lòng liên hệ Ban Quản Lý.";
            return RedirectToPage("/Resident/Index");
        }

        if (!Input.ApartmentId.HasValue ||
            !await _residentApartmentAccessService.IsResidentInApartmentAsync(userId, Input.ApartmentId.Value))
        {
            ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.ApartmentId)}", "Căn hộ đã chọn không hợp lệ.");
        }

        ValidateAttachments();

        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync(userId);
            return Page();
        }

        var request = new Request
        {
            ApartmentId = Input.ApartmentId!.Value,
            ResidentId = userId,
            Title = Input.Title,
            RequestType = Input.RequestType,
            Priority = RequestPriority.Normal,
            Description = Input.Description,
        };

        try
        {
            await _requestService.CreateRequestAsync(request, Attachments);
            TempData["SuccessMessage"] = $"Yêu cầu #{request.RequestNumber} đã được gửi thành công.";
            return RedirectToPage("MyRequests");
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi gửi yêu cầu. Vui lòng thử lại.");
            await LoadSelectListsAsync(userId);
            return Page();
        }
    }

    private void ValidateAttachments()
    {
        if (Attachments == null || Attachments.Count == 0) return;

        if (Attachments.Count > 5)
        {
            ModelState.AddModelError(nameof(Attachments), "Tối đa 5 tệp đính kèm.");
            return;
        }

        foreach (var file in Attachments)
        {
            if (file.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError(nameof(Attachments),
                    $"Tệp '{file.FileName}' vượt quá dung lượng tối đa 5MB.");
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                ModelState.AddModelError(nameof(Attachments),
                    $"Tệp '{file.FileName}' không hợp lệ. Chỉ chấp nhận ảnh (JPG, PNG, GIF, WEBP) và PDF.");
            }
        }
    }

    private async Task LoadSelectListsAsync(int userId)
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

        RequestTypeOptions = new SelectList(
            Enum.GetValues<RequestType>().Select(e => new
            {
                Value = e.ToString(),
                Text = e.GetDisplayName()
            }),
            "Value", "Text", Input.RequestType?.ToString());
    }
}
