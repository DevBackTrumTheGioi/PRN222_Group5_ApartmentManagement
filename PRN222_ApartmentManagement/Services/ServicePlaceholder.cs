namespace PRN222_ApartmentManagement.Services;

/// <summary>
/// Ví dụ cách sử dụng Activity Log Service trong Repository/PageModel
/// </summary>
public class ActivityLogUsageExamples
{
    /*
     * ============================================================
     * CÁCH SỬ DỤNG ACTIVITY LOG SERVICE
     * ============================================================
     * 
     * 1. INJECT SERVICE VÀO CONSTRUCTOR
     * ----------------------------------
     */

    // Ví dụ trong Repository:
    /*
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly ApartmentDbContext _context;
        private readonly IActivityLogService _activityLog;

        public ApartmentRepository(ApartmentDbContext context, IActivityLogService activityLog)
        {
            _context = context;
            _activityLog = activityLog;
        }

        public async Task<Apartment> CreateAsync(Apartment apartment)
        {
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();

            // Log hành động CREATE
            await _activityLog.LogCreateAsync(
                entityName: "Apartment",
                entityId: apartment.ApartmentId.ToString(),
                newValues: apartment,
                description: $"Tạo căn hộ mới: {apartment.ApartmentNumber}"
            );

            return apartment;
        }

        public async Task<Apartment> UpdateAsync(Apartment apartment)
        {
            var oldApartment = await _context.Apartments.AsNoTracking()
                .FirstOrDefaultAsync(a => a.ApartmentId == apartment.ApartmentId);

            _context.Apartments.Update(apartment);
            await _context.SaveChangesAsync();

            // Log hành động UPDATE với old values và new values
            await _activityLog.LogUpdateAsync(
                entityName: "Apartment",
                entityId: apartment.ApartmentId.ToString(),
                oldValues: oldApartment,
                newValues: apartment,
                description: $"Cập nhật căn hộ: {apartment.ApartmentNumber}"
            );

            return apartment;
        }

        public async Task DeleteAsync(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment != null)
            {
                _context.Apartments.Remove(apartment);
                await _context.SaveChangesAsync();

                // Log hành động DELETE
                await _activityLog.LogDeleteAsync(
                    entityName: "Apartment",
                    entityId: id.ToString(),
                    oldValues: apartment,
                    description: $"Xóa căn hộ: {apartment.ApartmentNumber}"
                );
            }
        }
    }
    */

    /*
     * 2. SỬ DỤNG TRONG RAZOR PAGE MODEL
     * ----------------------------------
     */

    // Ví dụ trong PageModel:
    /*
    public class CreateModel : PageModel
    {
        private readonly IApartmentRepository _apartmentRepo;
        private readonly IActivityLogService _activityLog;

        public CreateModel(IApartmentRepository apartmentRepo, IActivityLogService activityLog)
        {
            _apartmentRepo = apartmentRepo;
            _activityLog = activityLog;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var apartment = await _apartmentRepo.CreateAsync(Apartment);
                
                // Log đã được gọi trong repository, nhưng có thể log thêm ở đây
                await _activityLog.LogCustomAsync(
                    action: "Create",
                    entityName: "Apartment",
                    entityId: apartment.ApartmentId.ToString(),
                    description: "Admin tạo căn hộ mới từ trang web"
                );

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Log lỗi
                await _activityLog.LogErrorAsync(
                    action: "Create",
                    entityName: "Apartment",
                    entityId: null,
                    errorMessage: "Lỗi khi tạo căn hộ",
                    exception: ex
                );

                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo căn hộ");
                return Page();
            }
        }
    }
    */

    /*
     * 3. LOG AUTHENTICATION
     * ---------------------
     */

    // Trong Login page:
    /*
    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userRepository.GetByUsernameAsync(Input.Username);
        
        if (user != null && VerifyPassword(Input.Password, user.PasswordHash))
        {
            // Đăng nhập thành công
            await _activityLog.LogLoginAsync(
                userId: user.UserId,
                userName: user.FullName,
                isSuccess: true
            );
            
            return RedirectToPage("/Index");
        }
        else
        {
            // Đăng nhập thất bại
            await _activityLog.LogLoginAsync(
                userId: 0,
                userName: Input.Username,
                isSuccess: false,
                errorMessage: "Sai username hoặc password"
            );
            
            ModelState.AddModelError("", "Invalid login attempt");
            return Page();
        }
    }
    */

    /*
     * 4. XEM LOGS
     * -----------
     */

    // Trong Admin Log Viewer page:
    /*
    public class ActivityLogsModel : PageModel
    {
        private readonly IActivityLogService _activityLog;

        public ActivityLogsModel(IActivityLogService activityLog)
        {
            _activityLog = activityLog;
        }

        public List<ActivityLog> Logs { get; set; }

        public async Task OnGetAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? action = null,
            string? entityName = null,
            int pageNumber = 1)
        {
            Logs = await _activityLog.GetLogsAsync(
                fromDate: fromDate,
                toDate: toDate,
                action: action,
                entityName: entityName,
                pageNumber: pageNumber,
                pageSize: 50
            );
        }

        // Xem lịch sử của một entity cụ thể
        public async Task<IActionResult> OnGetEntityHistoryAsync(string entityName, string entityId)
        {
            var logs = await _activityLog.GetEntityLogsAsync(entityName, entityId);
            return new JsonResult(logs);
        }
    }
    */

    /*
     * 5. CLEANUP OLD LOGS (SCHEDULED TASK)
     * -------------------------------------
     */

    // Trong background service hoặc scheduled job:
    /*
    public class LogCleanupService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        public LogCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Chạy cleanup mỗi ngày lúc 2 giờ sáng
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            using var scope = _serviceProvider.CreateScope();
            var activityLog = scope.ServiceProvider.GetRequiredService<IActivityLogService>();
            
            // Xóa logs cũ hơn 90 ngày
            await activityLog.DeleteOldLogsAsync(daysToKeep: 90);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            return Task.CompletedTask;
        }
    }
    */

    /*
     * ============================================================
     * LƯU Ý QUAN TRỌNG
     * ============================================================
     * 
     * 1. Service được đăng ký là SCOPED - tự động dispose sau mỗi request
     * 
     * 2. KHÔNG dùng Singleton pattern thủ công - dùng DI container
     * 
     * 3. Service TỰ ĐỘNG lấy thông tin user hiện tại từ HttpContext
     * 
     * 4. Có thể log ngay cả khi CHƯA LOGIN (cho anonymous actions)
     * 
     * 5. Log KHÔNG làm crash app nếu có lỗi (try-catch bên trong)
     * 
     * 6. Old/New values được serialize thành JSON tự động
     * 
     * 7. IP Address và User Agent được tự động capture
     * 
     * ============================================================
     */
}