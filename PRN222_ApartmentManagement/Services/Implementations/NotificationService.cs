using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Hubs;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly ApartmentDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(ApartmentDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<(IList<Notification> Items, int TotalCount)> GetUserNotificationsAsync(
        int userId, int page = 1, int pageSize = 20, NotificationType? typeFilter = null)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId);

        if (typeFilter.HasValue)
        {
            query = query.Where(n => n.NotificationType == typeFilter.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<IList<Notification>> GetRecentNotificationsAsync(int userId, int count = 5)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

        if (notification == null) return false;
        if (notification.IsRead) return true;

        notification.IsRead = true;
        notification.ReadAt = DateTime.Now;
        await _context.SaveChangesAsync();

        // Push cập nhật unread count
        var unreadCount = await GetUnreadCountAsync(userId);
        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("UpdateUnreadCount", unreadCount);

        return true;
    }

    public async Task<int> MarkAllAsReadAsync(int userId)
    {
        var now = DateTime.Now;
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        if (unreadNotifications.Count == 0) return 0;

        foreach (var n in unreadNotifications)
        {
            n.IsRead = true;
            n.ReadAt = now;
        }

        await _context.SaveChangesAsync();

        // Push cập nhật unread count = 0
        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("UpdateUnreadCount", 0);

        return unreadNotifications.Count;
    }

    public async Task<Notification> CreateNotificationAsync(
        int userId,
        string title,
        string content,
        NotificationType notificationType = NotificationType.Other,
        ReferenceType referenceType = ReferenceType.None,
        int? referenceId = null,
        NotificationPriority priority = NotificationPriority.Normal)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Content = content,
            NotificationType = notificationType,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Priority = priority,
            IsRead = false,
            CreatedAt = DateTime.Now
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // Push real-time notification
        await PushNotificationAsync(userId, notification);

        return notification;
    }

    public async Task CreateBulkNotificationsAsync(
        IEnumerable<int> userIds,
        string title,
        string content,
        NotificationType notificationType = NotificationType.Other,
        ReferenceType referenceType = ReferenceType.None,
        int? referenceId = null,
        NotificationPriority priority = NotificationPriority.Normal)
    {
        var now = DateTime.Now;
        var notifications = new List<Notification>();

        foreach (var userId in userIds)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Content = content,
                NotificationType = notificationType,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Priority = priority,
                IsRead = false,
                CreatedAt = now
            };
            notifications.Add(notification);
        }

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync();

        // Push real-time cho từng user
        foreach (var notification in notifications)
        {
            await PushNotificationAsync(notification.UserId, notification);
        }
    }

    private async Task PushNotificationAsync(int userId, Notification notification)
    {
        var payload = new
        {
            notification.NotificationId,
            notification.Title,
            notification.Content,
            NotificationType = notification.NotificationType.ToString(),
            ReferenceType = notification.ReferenceType.ToString(),
            notification.ReferenceId,
            Priority = notification.Priority.ToString(),
            CreatedAt = notification.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
            notification.IsRead
        };

        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("ReceiveNotification", payload);

        var unreadCount = await GetUnreadCountAsync(userId);
        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("UpdateUnreadCount", unreadCount);
    }
}
