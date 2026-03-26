using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Visitors;

[Authorize(Roles = "Resident")]
public class CreateModel : PageModel
{
    private readonly IVisitorManagementService _visitorManagementService;

    public CreateModel(IVisitorManagementService visitorManagementService)
    {
        _visitorManagementService = visitorManagementService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new()
    {
        VisitDate = DateTime.Now.Date
    };

    public class InputModel
    {
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

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var result = await _visitorManagementService.CreateResidentVisitorAsync(
            userId.Value,
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

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
