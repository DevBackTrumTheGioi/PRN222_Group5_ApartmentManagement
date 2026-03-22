using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Requests;

[Authorize(Roles = "BQL_Manager")]
public class DetailsModel : PageModel
{
    private readonly IRequestService _requestService;

    public DetailsModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public Request Request { get; set; } = null!;

    [BindProperty]
    public CommentInput Input { get; set; } = new();

    public class CommentInput
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận.")]
        [MinLength(2, ErrorMessage = "Bình luận phải có ít nhất 2 ký tự.")]
        [MaxLength(1000, ErrorMessage = "Bình luận không được vượt quá 1000 ký tự.")]
        public string Content { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0) return NotFound();

        var request = await _requestService.GetRequestDetailAsync(id);
        if (request == null) return NotFound();

        // BQL không xem khiếu nại, trừ khi BQT đã forward cho manager này
        if (request.RequestType == RequestType.Complaint)
        {
            var userId = GetUserId();
            if (userId == null || request.EscalatedTo != userId.Value)
                return NotFound();
        }

        Request = request;
        return Page();
    }

    public async Task<IActionResult> OnPostCommentAsync(int id)
    {
        if (id <= 0) return NotFound();

        var userId = GetUserId();
        if (userId == null) return Forbid();

        var request = await _requestService.GetRequestDetailAsync(id);
        if (request == null) return NotFound();

        Request = request;

        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _requestService.AddCommentAsync(id, userId.Value, Input.Content);
            TempData["SuccessMessage"] = "Bình luận đã được gửi.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch
        {
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi gửi bình luận. Vui lòng thử lại.";
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, RequestStatus newStatus)
    {
        if (id <= 0) return NotFound();

        var request = await _requestService.GetRequestDetailAsync(id);
        if (request == null) return NotFound();

        // Manager chi duoc chuyen trang thai hop le
        var allowed = new[] {
            RequestStatus.InProgress,
            RequestStatus.Completed,
            RequestStatus.Cancelled,
            RequestStatus.Rejected
        };

        if (!allowed.Contains(newStatus))
        {
            TempData["ErrorMessage"] = "Trạng thái không hợp lệ.";
            return RedirectToPage(new { id });
        }

        try
        {
            await _requestService.UpdateStatusAsync(id, newStatus);
            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUpdatePriorityAsync(int id, RequestPriority newPriority)
    {
        if (id <= 0) return NotFound();

        if (!Enum.IsDefined(typeof(RequestPriority), newPriority))
        {
            TempData["ErrorMessage"] = "Mức độ ưu tiên không hợp lệ.";
            return RedirectToPage(new { id });
        }

        try
        {
            await _requestService.UpdatePriorityAsync(id, newPriority);
            TempData["SuccessMessage"] = "Đã cập nhật mức độ ưu tiên.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { id });
    }

    private int? GetUserId()
    {
        var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return str != null && int.TryParse(str, out var id) ? id : null;
    }
}
