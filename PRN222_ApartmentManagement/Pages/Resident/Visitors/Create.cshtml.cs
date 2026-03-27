using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Visitors;

[Authorize(Roles = "Resident")]
public class CreateModel : PageModel
{
    private readonly IVisitorManagementService _visitorManagementService;
    private readonly IResidentApartmentAccessService _residentApartmentAccessService;

    public CreateModel(
        IVisitorManagementService visitorManagementService,
        IResidentApartmentAccessService residentApartmentAccessService)
    {
        _visitorManagementService = visitorManagementService;
        _residentApartmentAccessService = residentApartmentAccessService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new()
    {
        VisitDate = DateTime.Now.Date
    };

    public SelectList ApartmentOptions { get; set; } = null!;

    public class InputModel
    {
        [Required(ErrorMessage = "Vui lòng chọn căn hộ áp dụng.")]
        [Display(Name = "Căn hộ áp dụng")]
        public int? ApartmentId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khách.")]
        [MaxLength(200, ErrorMessage = "Tên khách không vượt quá 200 ký tự.")]
        [Display(Name = "Tên khách")]
        public string VisitorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không vượt quá 20 ký tự.")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Số giấy tờ không vượt quá 20 ký tự.")]
        [Display(Name = "CCCD/CMND")]
        public string? IdentityCard { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày khách đến.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày khách đến")]
        public DateTime VisitDate { get; set; }

        [MaxLength(500, ErrorMessage = "Ghi chú không vượt quá 500 ký tự.")]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        await LoadApartmentOptionsAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        await LoadApartmentOptionsAsync(userId.Value);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _visitorManagementService.CreateResidentVisitorAsync(
            userId.Value,
            Input.ApartmentId!.Value,
            Input.VisitorName,
            Input.PhoneNumber,
            Input.IdentityCard,
            Input.VisitDate,
            Input.Notes);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToPage("Index");
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

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
