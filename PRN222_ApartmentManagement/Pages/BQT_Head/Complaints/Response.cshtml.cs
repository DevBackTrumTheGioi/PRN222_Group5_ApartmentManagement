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

namespace PRN222_ApartmentManagement.Pages.BQT_Head.Complaints;

[Authorize(Roles = "BQT_Head")]
public class ResponseModel : PageModel
{
    private readonly IRequestService _requestService;
    private readonly ApartmentDbContext _context;

    public ResponseModel(IRequestService requestService, ApartmentDbContext context)
    {
        _requestService = requestService;
        _context = context;
    }

    public Request Complaint { get; set; } = null!;
    public IReadOnlyList<SelectListItem> ManagerOptions { get; set; } = [];

    [BindProperty]
    public RespondInputModel RespondInput { get; set; } = new();

    [BindProperty]
    public ForwardInputModel ForwardInput { get; set; } = new();

    public class RespondInputModel
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung phản hồi.")]
        [MinLength(10, ErrorMessage = "Nội dung phản hồi phải có ít nhất 10 ký tự.")]
        [MaxLength(1000, ErrorMessage = "Nội dung phản hồi không vượt quá 1000 ký tự.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nội dung phản hồi không được chỉ chứa khoảng trắng.")]
        public string Content { get; set; } = string.Empty;
    }

    public class ForwardInputModel
    {
        [Required(ErrorMessage = "Vui lòng chọn quản lý BQL để chuyển xử lý.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn quản lý BQL để chuyển xử lý.")]
        public int ManagerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do chuyển xử lý.")]
        [MinLength(10, ErrorMessage = "Lý do chuyển xử lý phải có ít nhất 10 ký tự.")]
        [MaxLength(500, ErrorMessage = "Lý do chuyển xử lý không vượt quá 500 ký tự.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Lý do chuyển xử lý không được chỉ chứa khoảng trắng.")]
        public string Reason { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0) return NotFound();

        var complaint = await _requestService.GetRequestDetailAsync(id);
        if (complaint == null) return NotFound();
        if (complaint.RequestType != RequestType.Complaint)
        {
            TempData["ErrorMessage"] = "Yêu cầu này không phải khiếu nại gửi BQT.";
            return RedirectToPage("Index");
        }

        Complaint = complaint;
        await LoadManagerOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostRespondAsync(int id)
    {
        if (id <= 0) return NotFound();

        var userId = GetUserId();
        if (userId == null) return Forbid();

        var complaint = await _requestService.GetRequestDetailAsync(id);
        if (complaint == null) return NotFound();
        if (complaint.RequestType != RequestType.Complaint)
        {
            TempData["ErrorMessage"] = "Yêu cầu này không phải khiếu nại gửi BQT.";
            return RedirectToPage("Index");
        }

        if (complaint.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected)
        {
            TempData["ErrorMessage"] = "Không thể phản hồi khiếu nại đã đóng.";
            return RedirectToPage(new { id });
        }

        Complaint = complaint;

        // Clear toàn bộ rồi validate lại chỉ RespondInput
        ModelState.Clear();
        if (!TryValidateModel(RespondInput, nameof(RespondInput)))
        {
            await LoadManagerOptionsAsync();
            return Page();
        }

        try
        {
            var content = $"[BQT phản hồi] {RespondInput.Content.Trim()}";
            await _requestService.AddCommentAsync(id, userId.Value, content);
            TempData["SuccessMessage"] = "Đã gửi phản hồi cho cư dân thành công.";
            return RedirectToPage(new { id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi gửi phản hồi. Vui lòng thử lại.");
        }

        await LoadManagerOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostForwardAsync(int id)
    {
        if (id <= 0) return NotFound();

        var userId = GetUserId();
        if (userId == null) return Forbid();

        var complaint = await _requestService.GetRequestDetailAsync(id);
        if (complaint == null) return NotFound();
        if (complaint.RequestType != RequestType.Complaint)
        {
            TempData["ErrorMessage"] = "Yêu cầu này không phải khiếu nại gửi BQT.";
            return RedirectToPage("Index");
        }

        if (complaint.EscalatedAt.HasValue)
        {
            TempData["ErrorMessage"] = "Khiếu nại này đã được chuyển cho BQL trước đó.";
            return RedirectToPage(new { id });
        }

        if (complaint.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected)
        {
            TempData["ErrorMessage"] = "Không thể chuyển xử lý khiếu nại đã đóng.";
            return RedirectToPage(new { id });
        }

        Complaint = complaint;

        // Clear toàn bộ rồi validate lại chỉ ForwardInput
        ModelState.Clear();
        if (!TryValidateModel(ForwardInput, nameof(ForwardInput)))
        {
            await LoadManagerOptionsAsync();
            return Page();
        }

        var manager = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == ForwardInput.ManagerId
                                   && u.Role == UserRole.BQL_Manager
                                   && u.IsActive
                                   && !u.IsDeleted);

        if (manager == null)
        {
            ModelState.AddModelError($"{nameof(ForwardInput)}.{nameof(ForwardInput.ManagerId)}", "Quản lý BQL được chọn không hợp lệ hoặc không còn hoạt động.");
            await LoadManagerOptionsAsync();
            return Page();
        }

        try
        {
            await _requestService.ForwardComplaintAsync(id, ForwardInput.ManagerId, ForwardInput.Reason);

            var note = $"[BQT chuyển BQL] Chuyển cho {manager.FullName}. Lý do: {ForwardInput.Reason.Trim()}";
            await _requestService.AddCommentAsync(id, userId.Value, note);

            TempData["SuccessMessage"] = "Đã chuyển khiếu nại cho BQL Manager xử lý.";
            return RedirectToPage(new { id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi chuyển xử lý. Vui lòng thử lại.");
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
