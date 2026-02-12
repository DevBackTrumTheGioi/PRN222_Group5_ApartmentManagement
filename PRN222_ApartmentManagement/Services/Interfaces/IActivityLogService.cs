namespace PRN222_ApartmentManagement.Services;

/// <summary>
/// Interface cho Activity Logging Service
/// </summary>
public interface IActivityLogService
{
    /// <summary>
    /// Log hành động Create
    /// </summary>
    Task LogCreateAsync(string entityName, string entityId, object newValues, string? description = null);

    /// <summary>
    /// Log hành động Read (Query/View)
    /// </summary>
    Task LogReadAsync(string entityName, string? entityId = null, string? description = null);

    /// <summary>
    /// Log hành động Update
    /// </summary>
    Task LogUpdateAsync(string entityName, string entityId, object oldValues, object newValues, string? description = null);

    /// <summary>
    /// Log hành động Delete
    /// </summary>
    Task LogDeleteAsync(string entityName, string entityId, object oldValues, string? description = null);

    /// <summary>
    /// Log hành động Login
    /// </summary>
    Task LogLoginAsync(int userId, string userName, bool isSuccess, string? errorMessage = null);

    /// <summary>
    /// Log hành động Logout
    /// </summary>
    Task LogLogoutAsync(int userId, string userName);

    /// <summary>
    /// Log hành động tùy chỉnh
    /// </summary>
    Task LogCustomAsync(string action, string entityName, string? entityId = null, string? description = null, object? data = null);

    /// <summary>
    /// Log lỗi
    /// </summary>
    Task LogErrorAsync(string action, string entityName, string? entityId, string errorMessage, Exception? exception = null);

    /// <summary>
    /// Lấy logs theo filter
    /// </summary>
    Task<List<Models.ActivityLog>> GetLogsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int? userId = null,
        string? action = null,
        string? entityName = null,
        int pageNumber = 1,
        int pageSize = 50);

    /// <summary>
    /// Lấy logs của một entity cụ thể
    /// </summary>
    Task<List<Models.ActivityLog>> GetEntityLogsAsync(string entityName, string entityId);

    /// <summary>
    /// Lấy logs của một user
    /// </summary>
    Task<List<Models.ActivityLog>> GetUserLogsAsync(int userId, int pageNumber = 1, int pageSize = 50);

    /// <summary>
    /// Xóa logs cũ (cleanup)
    /// </summary>
    Task DeleteOldLogsAsync(int daysToKeep = 90);
}

