# Activity Log Service - Hướng Dẫn Sử Dụng

## 📋 Tổng Quan

Activity Log Service là một hệ thống ghi log tự động cho tất cả các hoạt động CRUD trong ứng dụng. Service này được triển khai với **Dependency Injection** (không dùng Singleton pattern thủ công).

## 🎯 Tại Sao Không Dùng Singleton?

### ❌ Không nên:
```csharp
// KHÔNG làm như này
public class ActivityLogService
{
    private static ActivityLogService? _instance;
    private static readonly object _lock = new object();
    
    public static ActivityLogService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new ActivityLogService();
                }
            }
            return _instance;
        }
    }
}
```

### ✅ Nên làm:
```csharp
// Đăng ký trong Program.cs
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();

// Inject vào constructor
public class ApartmentRepository
{
    private readonly IActivityLogService _activityLog;
    
    public ApartmentRepository(IActivityLogService activityLog)
    {
        _activityLog = activityLog;
    }
}
```

### Lý do:
1. **Thread-Safety**: DI Container đã xử lý thread-safety
2. **Testability**: Dễ dàng mock/test với DI
3. **Lifecycle Management**: ASP.NET Core tự động quản lý
4. **HttpContext Access**: Scoped service có thể truy cập HttpContext an toàn
5. **Best Practice**: Đúng chuẩn ASP.NET Core

## 🚀 Cài Đặt

### 1. Service đã được đăng ký tự động trong `Program.cs`:
```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
```

### 2. Chạy Migration:
```bash
dotnet ef database update
```

## 📝 Cách Sử Dụng

### 1. Trong Repository

```csharp
public class ApartmentRepository : IApartmentRepository
{
    private readonly ApartmentDbContext _context;
    private readonly IActivityLogService _activityLog;

    public ApartmentRepository(
        ApartmentDbContext context, 
        IActivityLogService activityLog)
    {
        _context = context;
        _activityLog = activityLog;
    }

    // CREATE
    public async Task<Apartment> CreateAsync(Apartment apartment)
    {
        _context.Apartments.Add(apartment);
        await _context.SaveChangesAsync();

        await _activityLog.LogCreateAsync(
            entityName: "Apartment",
            entityId: apartment.ApartmentId.ToString(),
            newValues: apartment,
            description: $"Tạo căn hộ: {apartment.ApartmentNumber}"
        );

        return apartment;
    }

    // UPDATE
    public async Task<Apartment> UpdateAsync(Apartment apartment)
    {
        var oldApartment = await _context.Apartments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.ApartmentId == apartment.ApartmentId);

        _context.Apartments.Update(apartment);
        await _context.SaveChangesAsync();

        await _activityLog.LogUpdateAsync(
            entityName: "Apartment",
            entityId: apartment.ApartmentId.ToString(),
            oldValues: oldApartment,
            newValues: apartment,
            description: $"Cập nhật căn hộ: {apartment.ApartmentNumber}"
        );

        return apartment;
    }

    // DELETE
    public async Task DeleteAsync(int id)
    {
        var apartment = await _context.Apartments.FindAsync(id);
        if (apartment != null)
        {
            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();

            await _activityLog.LogDeleteAsync(
                entityName: "Apartment",
                entityId: id.ToString(),
                oldValues: apartment,
                description: $"Xóa căn hộ: {apartment.ApartmentNumber}"
            );
        }
    }
}
```

### 2. Trong PageModel

```csharp
public class LoginModel : PageModel
{
    private readonly IUserRepository _userRepo;
    private readonly IActivityLogService _activityLog;

    public LoginModel(IUserRepository userRepo, IActivityLogService activityLog)
    {
        _userRepo = userRepo;
        _activityLog = activityLog;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userRepo.GetByUsernameAsync(Input.Username);
        
        if (user != null && VerifyPassword(Input.Password, user.PasswordHash))
        {
            // Log đăng nhập thành công
            await _activityLog.LogLoginAsync(
                userId: user.UserId,
                userName: user.FullName,
                isSuccess: true
            );
            
            return RedirectToPage("/Index");
        }
        
        // Log đăng nhập thất bại
        await _activityLog.LogLoginAsync(
            userId: 0,
            userName: Input.Username,
            isSuccess: false,
            errorMessage: "Sai username hoặc password"
        );
        
        return Page();
    }
}
```

### 3. Log Error

```csharp
try
{
    // Code có thể throw exception
    await ProcessSomething();
}
catch (Exception ex)
{
    await _activityLog.LogErrorAsync(
        action: "ProcessData",
        entityName: "Invoice",
        entityId: invoiceId.ToString(),
        errorMessage: "Lỗi khi xử lý hóa đơn",
        exception: ex
    );
    
    throw; // Re-throw nếu cần
}
```

### 4. Log Custom Action

```csharp
await _activityLog.LogCustomAsync(
    action: "ExportReport",
    entityName: "Invoice",
    entityId: null,
    description: "Xuất báo cáo tháng 12/2025",
    data: new { Month = 12, Year = 2025, RecordCount = 150 }
);
```

## 🔍 Xem Logs

### Trong Controller/PageModel:

```csharp
public class ActivityLogsModel : PageModel
{
    private readonly IActivityLogService _activityLog;

    public List<ActivityLog> Logs { get; set; }

    public async Task OnGetAsync(DateTime? fromDate, DateTime? toDate)
    {
        Logs = await _activityLog.GetLogsAsync(
            fromDate: fromDate ?? DateTime.Today.AddDays(-7),
            toDate: toDate ?? DateTime.Today,
            pageNumber: 1,
            pageSize: 50
        );
    }

    // Xem lịch sử của một entity cụ thể
    public async Task<IActionResult> OnGetEntityHistoryAsync(
        string entityName, 
        string entityId)
    {
        var logs = await _activityLog.GetEntityLogsAsync(entityName, entityId);
        return new JsonResult(logs);
    }
}
```

## 🧹 Cleanup Old Logs

### Tạo Background Service:

```csharp
public class LogCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LogCleanupService> _logger;

    public LogCleanupService(
        IServiceProvider serviceProvider,
        ILogger<LogCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var activityLog = scope.ServiceProvider
                    .GetRequiredService<IActivityLogService>();
                
                await activityLog.DeleteOldLogsAsync(daysToKeep: 90);
                _logger.LogInformation("Cleaned up old activity logs");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up logs");
            }

            // Chạy mỗi ngày
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}

// Đăng ký trong Program.cs
builder.Services.AddHostedService<LogCleanupService>();
```

## 📊 Dữ Liệu Được Ghi Log

| Field | Description |
|-------|-------------|
| `UserId` | ID của user thực hiện |
| `UserName` | Tên đầy đủ của user |
| `UserRole` | Role tại thời điểm thực hiện |
| `Action` | Create, Read, Update, Delete, Login, Logout |
| `EntityName` | Tên bảng (Apartment, User, Invoice...) |
| `EntityId` | ID của record |
| `Description` | Mô tả chi tiết |
| `OldValues` | Dữ liệu cũ (JSON) |
| `NewValues` | Dữ liệu mới (JSON) |
| `IpAddress` | IP của client |
| `UserAgent` | Browser info |
| `Timestamp` | Thời gian thực hiện |
| `IsSuccess` | Thành công hay không |
| `ErrorMessage` | Lỗi nếu có |

## ⚡ Tính Năng

### ✅ Auto-Capture
- Tự động lấy User từ HttpContext
- Tự động lấy IP Address
- Tự động lấy User Agent
- Tự động serialize object thành JSON

### ✅ Error Handling
- Không làm crash app nếu log fails
- Log cả errors vào database

### ✅ Performance
- Scoped service - tự động dispose
- Async/await cho tất cả operations
- Không block main thread

### ✅ Security
- Lưu trữ lịch sử ngay cả khi user bị xóa
- Audit trail đầy đủ
- Track IP và User Agent

## 🎓 Best Practices

1. **Luôn log trong Repository**, không log ở Controller/PageModel (trừ khi cần)
2. **Log cả thành công và thất bại**
3. **Viết Description rõ ràng**
4. **Cleanup logs định kỳ** (>90 ngày)
5. **Không log sensitive data** (password, credit card...)
6. **Test logging trong unit tests**

## 🔒 Security Notes

### Không log:
- Passwords
- Credit card numbers
- Personal identification numbers
- API keys/secrets

### Nên log:
- User actions
- CRUD operations
- Login/Logout
- Failed attempts
- Data changes (old → new)

## 📚 API Reference

```csharp
public interface IActivityLogService
{
    Task LogCreateAsync(string entityName, string entityId, object newValues, string? description = null);
    Task LogReadAsync(string entityName, string? entityId = null, string? description = null);
    Task LogUpdateAsync(string entityName, string entityId, object oldValues, object newValues, string? description = null);
    Task LogDeleteAsync(string entityName, string entityId, object oldValues, string? description = null);
    Task LogLoginAsync(int userId, string userName, bool isSuccess, string? errorMessage = null);
    Task LogLogoutAsync(int userId, string userName);
    Task LogCustomAsync(string action, string entityName, string? entityId = null, string? description = null, object? data = null);
    Task LogErrorAsync(string action, string entityName, string? entityId, string errorMessage, Exception? exception = null);
    Task<List<ActivityLog>> GetLogsAsync(...);
    Task<List<ActivityLog>> GetEntityLogsAsync(string entityName, string entityId);
    Task<List<ActivityLog>> GetUserLogsAsync(int userId, int pageNumber = 1, int pageSize = 50);
    Task DeleteOldLogsAsync(int daysToKeep = 90);
}
```

## 🏁 Kết Luận

Activity Log Service cung cấp một cách **an toàn, hiệu quả và dễ sử dụng** để theo dõi tất cả hoạt động trong hệ thống. Việc sử dụng **Dependency Injection** thay vì Singleton pattern đảm bảo code sạch, dễ test và tuân theo best practices của ASP.NET Core.

