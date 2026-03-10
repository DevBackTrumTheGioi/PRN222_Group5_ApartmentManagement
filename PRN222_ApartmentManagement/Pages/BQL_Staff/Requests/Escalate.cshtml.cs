using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.Requests;

[Authorize(Roles = "BQL_Staff")]
public class EscalateModel : PageModel
{
    private readonly IRequestService _requestService;
    private readonly ApartmentDbContext _context;

    public EscalateModel(IRequestService requestService, ApartmentDbContext context)
    {
        _requestService = requestService;
        _context = context;
    }

    public Request Request { get; set; } = null!;

    public IReadOnlyList<SelectListItem> ManagerOptions { get; set; } = [];

    [BindProperty]
    public EscalateInput Input { get; set; } = new();

    public class EscalateInput
    {
        [Required(ErrorMessage = "Vui lòng chọn quản lý nhận escalation.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn quản lý nhận escalation.")]
        public int ManagerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do chuyển cấp.")]
        [MinLength(10, ErrorMessage = "Lý do phải có ít nhất 10 ký tự.")]
        [MaxLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Lý do không được chỉ chứa khoảng trắng.")]
        public string Reason { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0) return NotFound();

        var userId = GetUserId();
        if (userId == null) return Forbid();

        var request = await _requestService.GetRequestDetailAsync(id);
        if (request == null) return NotFound();

        // Staff chỉ escalate yêu cầu được giao cho mình
        if (request.AssignedTo != userId.Value) return Forbid();

        if (request.EscalatedAt.HasValue)
        {
            TempData["ErrorMessage"] = "Yêu cầu này đã được chuyển cấp trước đó.";
            return RedirectToPage("Assigned");
        }

        if (request.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected)
        {
            TempData["ErrorMessage"] = "Không thể chuyển cấp yêu cầu đã đóng.";
            return RedirectToPage("Assigned");
        }

        Request = request;
        await LoadManagerOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id <= 0) return NotFound();

        var userId = GetUserId();
        if (userId == null) return Forbid();

        var request = await _requestService.GetRequestDetailAsync(id);
        if (request == null) return NotFound();
        if (request.AssignedTo != userId.Value) return Forbid();

        Request = request;

        if (!ModelState.IsValid)
        {
            await LoadManagerOptionsAsync();
            return Page();
        }

        try
        {
            await _requestService.EscalateAsync(id, Input.ManagerId, Input.Reason);
            TempData["SuccessMessage"] = "Đã chuyển cấp yêu cầu thành công. Quản lý sẽ tiếp nhận và xử lý.";
            return RedirectToPage("Assigned");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi. Vui lòng thử lại.");
        }

        await LoadManagerOptionsAsync();
        return Page();
    }

    private async Task LoadManagerOptionsAsync()
    {
        var managers = await _context.Users
            .Where(u => u.Role == UserRole.BQL_Manager && u.IsActive && !u.IsDeleted)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        ManagerOptions = managers
            .Select(m => new SelectListItem(m.FullName, m.UserId.ToString()))
            .ToList();
    }

    private int? GetUserId()
    {
        var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return str != null && int.TryParse(str, out var id) ? id : null;
    }
}
