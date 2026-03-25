using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Requests;

[Authorize(Roles = "Resident")]
public class DetailsModel : PageModel
{
    private readonly IRequestService _requestService;
    private readonly INotificationService _notificationService;
    private readonly ApartmentDbContext _context;

    public DetailsModel(IRequestService requestService, INotificationService notificationService, ApartmentDbContext context)
    {
        _requestService = requestService;
        _notificationService = notificationService;
        _context = context;
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
            var trimmedContent = Input.Content.Trim();
            await _requestService.AddCommentAsync(id, userId.Value, trimmedContent);

            await NotifyComplaintHandlerAsync(request);

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

    private async Task NotifyComplaintHandlerAsync(Request request)
    {
        if (request.RequestType != RequestType.Complaint)
        {
            await NotifyNonComplaintHandlerAsync(request);
            return;
        }

        if (request.EscalatedTo.HasValue)
        {
            await _notificationService.CreateNotificationAsync(
                request.EscalatedTo.Value,
                "Cư dân đã phản hồi khiếu nại",
                $"Cư dân vừa phản hồi thêm cho khiếu nại {request.RequestNumber}.",
                NotificationType.Request,
                ReferenceType.Request,
                request.RequestId,
                NotificationPriority.High);
            return;
        }

        var bqtHeadIds = await _context.Users
            .Where(u => u.Role == UserRole.BQT_Head && u.IsActive && !u.IsDeleted)
            .Select(u => u.UserId)
            .ToListAsync();

        foreach (var bqtHeadId in bqtHeadIds)
        {
            await _notificationService.CreateNotificationAsync(
                bqtHeadId,
                "Cư dân đã phản hồi khiếu nại",
                $"Cư dân vừa phản hồi thêm cho khiếu nại {request.RequestNumber}.",
                NotificationType.Request,
                ReferenceType.Request,
                request.RequestId,
                NotificationPriority.High);
        }
    }

    private async Task NotifyNonComplaintHandlerAsync(Request request)
    {
        var recipientIds = new HashSet<int>();

        if (request.AssignedTo.HasValue)
        {
            recipientIds.Add(request.AssignedTo.Value);
        }

        if (request.EscalatedTo.HasValue)
        {
            recipientIds.Add(request.EscalatedTo.Value);
        }

        if (recipientIds.Count == 0)
        {
            var managerIds = await _context.Users
                .Where(u => u.Role == UserRole.BQL_Manager && u.IsActive && !u.IsDeleted)
                .Select(u => u.UserId)
                .ToListAsync();

            foreach (var managerId in managerIds)
            {
                recipientIds.Add(managerId);
            }
        }

        foreach (var recipientId in recipientIds)
        {
            await _notificationService.CreateNotificationAsync(
                recipientId,
                "Cư dân đã phản hồi yêu cầu",
                $"Cư dân vừa phản hồi thêm cho yêu cầu {request.RequestNumber}.",
                NotificationType.Request,
                ReferenceType.Request,
                request.RequestId,
                NotificationPriority.Normal);
        }
    }

    private int? GetUserId()
    {
        var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return str != null && int.TryParse(str, out var id) ? id : null;
    }
}
