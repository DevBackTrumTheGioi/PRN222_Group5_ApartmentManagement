using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Services.Implementations;

/// <summary>
/// Service ghi log các hoạt động CRUD trong hệ thống
/// Đăng ký với DI Container như Scoped service
/// </summary>
public class ActivityLogService : IActivityLogService
{
    private readonly ApartmentDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ActivityLogService> _logger;

    public ActivityLogService(
        ApartmentDbContext context,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ActivityLogService> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    #region Core Logging Methods

    public async Task LogCreateAsync(string entityName, string entityId, object newValues, string? description = null)
    {
        await CreateLogAsync("Create", entityName, entityId, description, null, newValues);
    }

    public async Task LogReadAsync(string entityName, string? entityId = null, string? description = null)
    {
        await CreateLogAsync("Read", entityName, entityId, description, null, null);
    }

    public async Task LogUpdateAsync(string entityName, string entityId, object oldValues, object newValues, string? description = null)
    {
        await CreateLogAsync("Update", entityName, entityId, description, oldValues, newValues);
    }

    public async Task LogDeleteAsync(string entityName, string entityId, object oldValues, string? description = null)
    {
        await CreateLogAsync("Delete", entityName, entityId, description, oldValues, null);
    }

    public async Task LogLoginAsync(int userId, string userName, bool isSuccess, string? errorMessage = null)
    {
        await CreateLogAsync(
            action: "Login",
            entityName: "Authentication",
            entityId: userId.ToString(),
            description: isSuccess ? $"User {userName} đăng nhập thành công" : $"User {userName} đăng nhập thất bại",
            oldValues: null,
            newValues: null,
            isSuccess: isSuccess,
            errorMessage: errorMessage,
            overrideUserId: userId,
            overrideUserName: userName
        );
    }

    public async Task LogLogoutAsync(int userId, string userName)
    {
        await CreateLogAsync(
            action: "Logout",
            entityName: "Authentication",
            entityId: userId.ToString(),
            description: $"User {userName} đăng xuất",
            oldValues: null,
            newValues: null,
            overrideUserId: userId,
            overrideUserName: userName
        );
    }

    public async Task LogCustomAsync(string action, string entityName, string? entityId = null, string? description = null, object? data = null)
    {
        await CreateLogAsync(action, entityName, entityId, description, null, data);
    }

    public async Task LogErrorAsync(string action, string entityName, string? entityId, string errorMessage, Exception? exception = null)
    {
        var fullErrorMessage = exception != null
            ? $"{errorMessage} - Exception: {exception.Message}\nStackTrace: {exception.StackTrace}"
            : errorMessage;

        await CreateLogAsync(
            action: action,
            entityName: entityName,
            entityId: entityId,
            description: $"Error: {errorMessage}",
            oldValues: null,
            newValues: exception != null ? new { ExceptionType = exception.GetType().Name, Message = exception.Message } : null,
            isSuccess: false,
            errorMessage: fullErrorMessage
        );
    }

    #endregion

    #region Query Methods

    public async Task<List<ActivityLog>> GetLogsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int? userId = null,
        string? action = null,
        string? entityName = null,
        int pageNumber = 1,
        int pageSize = 50)
    {
        var query = _context.ActivityLogs.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(l => l.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(l => l.Timestamp <= toDate.Value);

        if (userId.HasValue)
            query = query.Where(l => l.UserId == userId.Value);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(l => l.Action == action);

        if (!string.IsNullOrEmpty(entityName))
            query = query.Where(l => l.EntityName == entityName);

        return await query
            .OrderByDescending(l => l.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(l => l.User)
            .ToListAsync();
    }

    public async Task<List<ActivityLog>> GetEntityLogsAsync(string entityName, string entityId)
    {
        return await _context.ActivityLogs
            .Where(l => l.EntityName == entityName && l.EntityId == entityId)
            .OrderByDescending(l => l.Timestamp)
            .Include(l => l.User)
            .ToListAsync();
    }

    public async Task<List<ActivityLog>> GetUserLogsAsync(int userId, int pageNumber = 1, int pageSize = 50)
    {
        return await _context.ActivityLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(l => l.User)
            .ToListAsync();
    }

    public async Task DeleteOldLogsAsync(int daysToKeep = 90)
    {
        var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
        var oldLogs = await _context.ActivityLogs
            .Where(l => l.Timestamp < cutoffDate)
            .ToListAsync();

        if (oldLogs.Any())
        {
            _context.ActivityLogs.RemoveRange(oldLogs);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Deleted {oldLogs.Count} old activity logs (older than {daysToKeep} days)");
        }
    }

    #endregion

    #region Private Helper Methods

    private async Task CreateLogAsync(
        string action,
        string entityName,
        string? entityId,
        string? description,
        object? oldValues,
        object? newValues,
        bool isSuccess = true,
        string? errorMessage = null,
        int? overrideUserId = null,
        string? overrideUserName = null)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var currentUser = await GetCurrentUserAsync();

            var log = new ActivityLog
            {
                UserId = overrideUserId ?? currentUser?.UserId,
                UserName = overrideUserName ?? currentUser?.FullName ?? "System",
                UserRole = currentUser?.Role?.ToString() ?? "System",
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Description = description,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues, GetJsonOptions()) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues, GetJsonOptions()) : null,
                IpAddress = GetClientIpAddress(httpContext),
                UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                Timestamp = DateTime.Now,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage
            };

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Không để lỗi logging làm crash ứng dụng
            _logger.LogError(ex, $"Failed to create activity log for {action} on {entityName}");
        }
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return await _context.Users.FindAsync(userId);
            }
        }
        return null;
    }

    private string? GetClientIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null) return null;

        // Check for forwarded IP first (if behind proxy/load balancer)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };
    }

    #endregion
}

