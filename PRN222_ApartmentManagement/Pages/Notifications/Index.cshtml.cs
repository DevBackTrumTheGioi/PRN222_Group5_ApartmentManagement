using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Notifications;

[Authorize]
public class IndexModel : PageModel
{
    private readonly INotificationService _notificationService;

    public IndexModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public IList<NotificationItemViewModel> Notifications { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public string? CurrentTypeFilter { get; set; }
    public const int PageSize = 15;

    public class NotificationItemViewModel
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string NotificationTypeDisplay { get; set; } = string.Empty;
        public string ReferenceType { get; set; } = string.Empty;
        public int? ReferenceId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Icon { get; set; } = "🔔";
        public string TimeAgo { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = "/Notifications";
    }

    public async Task<IActionResult> OnGetAsync(int page = 1, string? type = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        CurrentPage = page < 1 ? 1 : page;
        CurrentTypeFilter = type;

        NotificationType? typeFilter = null;
        if (!string.IsNullOrEmpty(type) && Enum.TryParse<NotificationType>(type, out var parsed))
        {
            typeFilter = parsed;
        }

        var (items, totalCount) = await _notificationService.GetUserNotificationsAsync(
            userId.Value, CurrentPage, PageSize, typeFilter);

        var role = User.FindFirstValue(ClaimTypes.Role);

        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
        UnreadCount = await _notificationService.GetUnreadCountAsync(userId.Value);

        Notifications = items.Select(n => new NotificationItemViewModel
        {
            NotificationId = n.NotificationId,
            Title = n.Title,
            Content = n.Content,
            NotificationType = n.NotificationType.ToString(),
            NotificationTypeDisplay = GetTypeDisplayName(n.NotificationType),
            ReferenceType = n.ReferenceType.ToString(),
            ReferenceId = n.ReferenceId,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            Priority = n.Priority.ToString(),
            CreatedAt = n.CreatedAt,
            Icon = NotificationUtils.GetNotificationIcon(n.NotificationType.ToString()),
            TimeAgo = GetTimeAgo(n.CreatedAt),
            RedirectUrl = NotificationUtils.GetNotificationRedirectUrl(
                n.NotificationType.ToString(),
                n.ReferenceType.ToString(),
                n.ReferenceId,
                role)
        }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnGetUnreadCountAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return new JsonResult(new { count = 0 });
        }

        var count = await _notificationService.GetUnreadCountAsync(userId.Value);
        return new JsonResult(new { count });
    }

    public async Task<IActionResult> OnGetRecentAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return new JsonResult(new { items = Array.Empty<object>() });
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        var items = await _notificationService.GetRecentNotificationsAsync(userId.Value, 5);
        var result = items.Select(n => new
        {
            n.NotificationId,
            n.Title,
            n.Content,
            NotificationType = n.NotificationType.ToString(),
            ReferenceType = n.ReferenceType.ToString(),
            n.ReferenceId,
            n.IsRead,
            Priority = n.Priority.ToString(),
            CreatedAt = n.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
            Icon = NotificationUtils.GetNotificationIcon(n.NotificationType.ToString()),
            TimeAgo = GetTimeAgo(n.CreatedAt),
            RedirectUrl = NotificationUtils.GetNotificationRedirectUrl(
                n.NotificationType.ToString(),
                n.ReferenceType.ToString(),
                n.ReferenceId,
                role)
        });

        return new JsonResult(new { items = result });
    }

    public async Task<IActionResult> OnPostMarkAsReadAsync(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        await _notificationService.MarkAsReadAsync(id, userId.Value);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return new JsonResult(new { success = true });
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostMarkAllReadAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var count = await _notificationService.MarkAllAsReadAsync(userId.Value);
        TempData["SuccessMessage"] = $"Đã đánh dấu {count} thông báo là đã đọc.";

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return new JsonResult(new { success = true, count });
        }

        return RedirectToPage();
    }

    private int? GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var userId) ? userId : null;
    }

    private static string GetTypeDisplayName(NotificationType type)
    {
        return type switch
        {
            NotificationType.System => "Hệ thống",
            NotificationType.Invoice => "Hóa đơn",
            NotificationType.Request => "Yêu cầu",
            NotificationType.Announcement => "Thông báo chung",
            NotificationType.Contract => "Hợp đồng",
            NotificationType.Amenity => "Tiện ích",
            NotificationType.Community => "Khảo sát & bỏ phiếu",
            _ => "Khác"
        };
    }

    private static string GetTimeAgo(DateTime createdAt)
    {
        var diff = DateTime.Now - createdAt;
        if (diff.TotalMinutes < 1) return "Vừa xong";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} phút trước";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} giờ trước";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} ngày trước";
        if (diff.TotalDays < 30) return $"{(int)(diff.TotalDays / 7)} tuần trước";
        return createdAt.ToString("dd/MM/yyyy");
    }
}
