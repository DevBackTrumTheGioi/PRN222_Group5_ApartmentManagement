using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Requests;

[Authorize(Roles = "Resident")]
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
        [MaxLength(1000, ErrorMessage = "Bình luận không vượt quá 1000 ký tự.")]
        public string Content { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0) return NotFound();

        var userId = GetUserId();
        if (userId == null) return Forbid();

        var request = await _requestService.GetRequestDetailAsync(id);
        if (request == null) return NotFound();

        // Resident chỉ xem được yêu cầu của mình
        if (request.ResidentId != userId.Value) return Forbid();

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
        if (request.ResidentId != userId.Value) return Forbid();

        // Gán trước để dùng nếu cần return Page()
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

    private int? GetUserId()
    {
        var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return str != null && int.TryParse(str, out var id) ? id : null;
    }
}
