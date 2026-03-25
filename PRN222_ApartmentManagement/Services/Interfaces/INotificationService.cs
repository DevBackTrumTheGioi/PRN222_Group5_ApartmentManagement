using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// Lấy danh sách notification phân trang cho user
    /// </summary>
    Task<(IList<Notification> Items, int TotalCount)> GetUserNotificationsAsync(
        int userId, int page = 1, int pageSize = 20, NotificationType? typeFilter = null);

    /// <summary>
    /// Lấy số notification chưa đọc
    /// </summary>
    Task<int> GetUnreadCountAsync(int userId);

    /// <summary>
    /// Lấy N notification mới nhất (cho dropdown)
    /// </summary>
    Task<IList<Notification>> GetRecentNotificationsAsync(int userId, int count = 5);

    /// <summary>
    /// Đánh dấu một notification đã đọc
    /// </summary>
    Task<bool> MarkAsReadAsync(int notificationId, int userId);

    /// <summary>
    /// Đánh dấu tất cả notification đã đọc
    /// </summary>
    Task<int> MarkAllAsReadAsync(int userId);

    /// <summary>
    /// Tạo notification + push real-time qua SignalR
    /// </summary>
    Task<Notification> CreateNotificationAsync(
        int userId,
        string title,
        string content,
        NotificationType notificationType = NotificationType.Other,
        ReferenceType referenceType = ReferenceType.None,
        int? referenceId = null,
        NotificationPriority priority = NotificationPriority.Normal);

    /// <summary>
    /// Tạo notification cho nhiều user (bulk) + push real-time
    /// </summary>
    Task CreateBulkNotificationsAsync(
        IEnumerable<int> userIds,
        string title,
        string content,
        NotificationType notificationType = NotificationType.Other,
        ReferenceType referenceType = ReferenceType.None,
        int? referenceId = null,
        NotificationPriority priority = NotificationPriority.Normal);
}
