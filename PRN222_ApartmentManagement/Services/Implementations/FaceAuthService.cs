using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class FaceAuthService : IFaceAuthService
{
    private const int ExpectedDescriptorLength = 128;
    private const double FaceMatchThreshold = 0.60;

    private readonly ApartmentDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<FaceAuthHistory> _faceAuthHistoryRepository;
    private readonly IActivityLogService _activityLogService;

    public FaceAuthService(
        ApartmentDbContext context,
        IUserRepository userRepository,
        IGenericRepository<FaceAuthHistory> faceAuthHistoryRepository,
        IActivityLogService activityLogService)
    {
        _context = context;
        _userRepository = userRepository;
        _faceAuthHistoryRepository = faceAuthHistoryRepository;
        _activityLogService = activityLogService;
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterFaceAsync(int userId, string faceDescriptor)
    {
        return await RegisterFaceInternalAsync(userId, userId, faceDescriptor, isStaffAssisted: false);
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterFaceForResidentAsync(int actorUserId, int residentId, string faceDescriptor)
    {
        return await RegisterFaceInternalAsync(actorUserId, residentId, faceDescriptor, isStaffAssisted: true);
    }

    public async Task<(bool Success, string? ErrorMessage)> ResetFaceAsync(int actorUserId, int residentId)
    {
        var user = await GetResidentByIdAsync(residentId);
        if (user == null)
        {
            return (false, "Không tìm thấy cư dân.");
        }

        if (!user.IsFaceRegistered && string.IsNullOrWhiteSpace(user.FaceDescriptor))
        {
            return (false, "Cư dân này chưa có dữ liệu Face ID để xóa.");
        }

        user.FaceDescriptor = null;
        user.IsFaceRegistered = false;
        user.UpdatedAt = DateTime.Now;

        await _userRepository.UpdateAsync(user);
        await _activityLogService.LogCustomAsync(
            action: "ResetFaceData",
            entityName: nameof(User),
            entityId: user.UserId.ToString(),
            description: $"Reset dữ liệu Face ID cho cư dân {user.FullName}",
            data: new { ResidentId = user.UserId, ActorUserId = actorUserId });

        return (true, null);
    }

    public async Task<User?> GetResidentByIdAsync(int userId)
    {
        var user = await _userRepository.GetActiveByIdAsync(userId);
        if (user == null || user.Role != UserRole.Resident)
        {
            return null;
        }

        return user;
    }

    public async Task<User?> GetResidentForManagementAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Apartment)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted && u.Role == UserRole.Resident);
    }

    public async Task<List<FaceAuthHistory>> GetRecentHistoriesAsync(int userId, int take)
    {
        var histories = await _faceAuthHistoryRepository.FindAsync(h => h.ResidentId == userId);
        return histories.OrderByDescending(h => h.AuthTime).Take(take).ToList();
    }

    public async Task<List<FaceAuthHistory>> GetHistoriesAsync(int userId)
    {
        var histories = await _faceAuthHistoryRepository.FindAsync(h => h.ResidentId == userId);
        return histories.OrderByDescending(h => h.AuthTime).ToList();
    }

    public async Task<List<FaceResidentSummaryDto>> GetResidentSummariesAsync(string? searchTerm, bool? isRegistered)
    {
        var residentQuery = _context.Users
            .AsNoTracking()
            .Include(u => u.Apartment)
            .Where(u => !u.IsDeleted && u.Role == UserRole.Resident);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim();
            residentQuery = residentQuery.Where(u =>
                u.FullName.Contains(keyword) ||
                u.Username.Contains(keyword) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(keyword)) ||
                (u.Apartment != null && u.Apartment.ApartmentNumber.Contains(keyword)));
        }

        if (isRegistered.HasValue)
        {
            residentQuery = residentQuery.Where(u => u.IsFaceRegistered == isRegistered.Value);
        }

        var residents = await residentQuery
            .OrderByDescending(u => u.IsFaceRegistered)
            .ThenBy(u => u.FullName)
            .ToListAsync();

        var residentIds = residents.Select(u => u.UserId).ToList();
        var histories = residentIds.Count == 0
            ? new List<FaceAuthHistory>()
            : await _context.FaceAuthHistories
                .AsNoTracking()
                .Where(h => residentIds.Contains(h.ResidentId))
                .ToListAsync();

        return residents.Select(user =>
        {
            var residentHistories = histories.Where(h => h.ResidentId == user.UserId).ToList();
            return new FaceResidentSummaryDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Username = user.Username,
                ApartmentNumber = user.Apartment?.ApartmentNumber,
                BuildingBlock = user.Apartment?.BuildingBlock,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                IsFaceRegistered = user.IsFaceRegistered && !string.IsNullOrWhiteSpace(user.FaceDescriptor),
                RegisteredAt = user.IsFaceRegistered ? user.UpdatedAt ?? user.CreatedAt : null,
                LastAuthTime = residentHistories.OrderByDescending(h => h.AuthTime).Select(h => (DateTime?)h.AuthTime).FirstOrDefault(),
                SuccessfulAuthCount = residentHistories.Count(h => h.IsSuccess),
                FailedAuthCount = residentHistories.Count(h => !h.IsSuccess)
            };
        }).ToList();
    }

    public async Task<List<FaceAuthLogDto>> GetManagementLogsAsync(string? searchTerm, bool? isSuccess, DateTime? fromDate, DateTime? toDate, int take = 200)
    {
        var query = _context.FaceAuthHistories
            .AsNoTracking()
            .Include(h => h.Resident)
                .ThenInclude(r => r.Apartment)
            .Where(h => !h.Resident.IsDeleted && h.Resident.Role == UserRole.Resident);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim();
            query = query.Where(h =>
                h.Resident.FullName.Contains(keyword) ||
                h.Resident.Username.Contains(keyword) ||
                (h.Resident.Apartment != null && h.Resident.Apartment.ApartmentNumber.Contains(keyword)) ||
                (h.DeviceInfo != null && h.DeviceInfo.Contains(keyword)) ||
                (h.IpAddress != null && h.IpAddress.Contains(keyword)));
        }

        if (isSuccess.HasValue)
        {
            query = query.Where(h => h.IsSuccess == isSuccess.Value);
        }

        if (fromDate.HasValue)
        {
            var start = fromDate.Value.Date;
            query = query.Where(h => h.AuthTime >= start);
        }

        if (toDate.HasValue)
        {
            var end = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(h => h.AuthTime <= end);
        }

        return await query
            .OrderByDescending(h => h.AuthTime)
            .Take(take)
            .Select(h => new FaceAuthLogDto
            {
                HistoryId = h.HistoryId,
                ResidentId = h.ResidentId,
                ResidentName = h.Resident.FullName,
                ApartmentNumber = h.Resident.Apartment != null ? h.Resident.Apartment.ApartmentNumber : null,
                BuildingBlock = h.Resident.Apartment != null ? h.Resident.Apartment.BuildingBlock : null,
                AuthTime = h.AuthTime,
                IsSuccess = h.IsSuccess,
                ConfidenceScore = h.ConfidenceScore,
                IpAddress = h.IpAddress,
                DeviceInfo = h.DeviceInfo
            })
            .ToListAsync();
    }

    public async Task<FaceAuthDashboardDto> GetDashboardAsync(int recentDays = 30)
    {
        if (recentDays <= 0)
        {
            recentDays = 30;
        }

        var periodStart = DateTime.Now.Date.AddDays(-(recentDays - 1));
        var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var today = DateTime.Now.Date;

        var residentsQuery = _context.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted && u.Role == UserRole.Resident);

        var totalResidents = await residentsQuery.CountAsync();
        var registeredResidents = await residentsQuery.CountAsync(u => u.IsFaceRegistered && u.FaceDescriptor != null);
        var historiesInPeriod = await _context.FaceAuthHistories
            .AsNoTracking()
            .Where(h => h.AuthTime >= periodStart)
            .ToListAsync();

        var attemptsToday = await _context.FaceAuthHistories
            .AsNoTracking()
            .CountAsync(h => h.AuthTime >= today);

        return new FaceAuthDashboardDto
        {
            TotalResidents = totalResidents,
            RegisteredResidents = registeredResidents,
            UnregisteredResidents = Math.Max(totalResidents - registeredResidents, 0),
            RegistrationRate = totalResidents == 0 ? 0 : Math.Round((decimal)registeredResidents * 100 / totalResidents, 2),
            RegisteredThisMonth = await residentsQuery.CountAsync(u => u.IsFaceRegistered && (u.UpdatedAt ?? u.CreatedAt) >= monthStart),
            AttemptsToday = attemptsToday,
            AttemptsInPeriod = historiesInPeriod.Count,
            SuccessfulAttemptsInPeriod = historiesInPeriod.Count(h => h.IsSuccess),
            FailedAttemptsInPeriod = historiesInPeriod.Count(h => !h.IsSuccess),
            SuccessRateInPeriod = historiesInPeriod.Count == 0 ? 0 : Math.Round((decimal)historiesInPeriod.Count(h => h.IsSuccess) * 100 / historiesInPeriod.Count, 2),
            RecentDays = recentDays,
            RecentLogs = await GetManagementLogsAsync(null, null, periodStart, null, 8)
        };
    }

    public async Task<AmenityFaceAccessResultDto> ValidateAmenityAccessAsync(int amenityId, string faceDescriptor, string? ipAddress, string? deviceInfo)
    {
        var amenity = await GetAmenityForAccessAsync(amenityId);
        if (amenity == null)
        {
            return BuildAmenityErrorResult(amenityId, "Không tìm thấy tiện ích đang hoạt động.");
        }

        var validationError = ValidateDescriptor(faceDescriptor);
        if (validationError != null)
        {
            return BuildAmenityErrorResult(amenity.AmenityId, validationError, amenity.AmenityName, amenity.RequiresBooking);
        }

        var inputDescriptor = ParseDescriptor(faceDescriptor);
        if (inputDescriptor == null)
        {
            return BuildAmenityErrorResult(amenity.AmenityId, "Không đọc được dữ liệu khuôn mặt để so sánh.", amenity.AmenityName, amenity.RequiresBooking);
        }

        var residents = await _context.Users
            .AsNoTracking()
            .Include(u => u.Apartment)
            .Where(u =>
                !u.IsDeleted &&
                u.IsActive &&
                u.Role == UserRole.Resident &&
                u.IsFaceRegistered &&
                u.FaceDescriptor != null)
            .ToListAsync();

        User? matchedResident = null;
        double bestDistance = double.MaxValue;

        foreach (var resident in residents)
        {
            var residentDescriptor = ParseDescriptor(resident.FaceDescriptor);
            if (residentDescriptor == null)
            {
                continue;
            }

            var distance = CalculateEuclideanDistance(inputDescriptor, residentDescriptor);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                matchedResident = resident;
            }
        }

        if (matchedResident == null || bestDistance > FaceMatchThreshold)
        {
            return new AmenityFaceAccessResultDto
            {
                CheckedAt = DateTime.Now,
                AmenityId = amenity.AmenityId,
                AmenityName = amenity.AmenityName,
                AmenityRequiresBooking = amenity.RequiresBooking,
                IsMatchFound = false,
                AccessGranted = false,
                IsWithinOperatingHours = IsAmenityOpenNow(amenity, DateTime.Now),
                Message = "Không tìm thấy cư dân phù hợp trong hệ thống Face ID.",
                AccessModeLabel = "Quét khuôn mặt"
            };
        }

        return await EvaluateAmenityAccessAsync(
            amenity,
            matchedResident,
            NormalizeConfidence(bestDistance),
            ipAddress,
            deviceInfo,
            isManualOverride: false,
            staffUserId: null);
    }

    public async Task<AmenityFaceAccessResultDto> ValidateAmenityAccessManualAsync(int amenityId, int residentId, int staffUserId, string? ipAddress, string? deviceInfo)
    {
        var amenity = await GetAmenityForAccessAsync(amenityId);
        if (amenity == null)
        {
            return BuildAmenityErrorResult(amenityId, "Không tìm thấy tiện ích đang hoạt động.");
        }

        var resident = await _context.Users
            .AsNoTracking()
            .Include(u => u.Apartment)
            .FirstOrDefaultAsync(u =>
                u.UserId == residentId &&
                !u.IsDeleted &&
                u.IsActive &&
                u.Role == UserRole.Resident);

        if (resident == null)
        {
            return BuildAmenityErrorResult(amenity.AmenityId, "Cư dân được chọn không hợp lệ.", amenity.AmenityName, amenity.RequiresBooking);
        }

        return await EvaluateAmenityAccessAsync(
            amenity,
            resident,
            confidenceScore: 1,
            ipAddress,
            deviceInfo,
            isManualOverride: true,
            staffUserId);
    }

    private async Task<(bool Success, string? ErrorMessage)> RegisterFaceInternalAsync(int actorUserId, int residentId, string faceDescriptor, bool isStaffAssisted)
    {
        var validationError = ValidateDescriptor(faceDescriptor);
        if (validationError != null)
        {
            return (false, validationError);
        }

        var user = await GetResidentByIdAsync(residentId);
        if (user == null)
        {
            return (false, "Không tìm thấy cư dân.");
        }

        user.FaceDescriptor = faceDescriptor;
        user.IsFaceRegistered = true;
        user.UpdatedAt = DateTime.Now;

        await _userRepository.UpdateAsync(user);
        await _activityLogService.LogCustomAsync(
            action: isStaffAssisted ? "RegisterFaceByStaff" : "RegisterFace",
            entityName: nameof(User),
            entityId: user.UserId.ToString(),
            description: isStaffAssisted
                ? $"Nhân viên hỗ trợ đăng ký Face ID cho cư dân {user.FullName}"
                : $"Cư dân {user.FullName} cập nhật Face ID",
            data: new { ResidentId = user.UserId, ActorUserId = actorUserId });

        return (true, null);
    }

    private async Task<AmenityFaceAccessResultDto> EvaluateAmenityAccessAsync(
        Amenity amenity,
        User resident,
        double confidenceScore,
        string? ipAddress,
        string? deviceInfo,
        bool isManualOverride,
        int? staffUserId)
    {
        var now = DateTime.Now;
        var isWithinOperatingHours = IsAmenityOpenNow(amenity, now);

        var result = new AmenityFaceAccessResultDto
        {
            CheckedAt = now,
            AmenityId = amenity.AmenityId,
            AmenityName = amenity.AmenityName,
            AmenityRequiresBooking = amenity.RequiresBooking,
            IsMatchFound = true,
            AccessGranted = false,
            IsManualOverride = isManualOverride,
            IsWithinOperatingHours = isWithinOperatingHours,
            ResidentId = resident.UserId,
            ResidentName = resident.FullName,
            ApartmentNumber = resident.Apartment?.ApartmentNumber,
            BuildingBlock = resident.Apartment?.BuildingBlock,
            ConfidenceScore = confidenceScore,
            AccessModeLabel = isManualOverride ? "Check-in thủ công" : "Quét khuôn mặt"
        };

        if (resident.ApartmentId == null)
        {
            result.Message = "Cư dân chưa được gán căn hộ nên không thể sử dụng tiện ích.";
            await LogFaceAccessAttemptAsync(resident.UserId, false, confidenceScore, ipAddress, BuildDeviceInfo(deviceInfo, amenity.AmenityName, isManualOverride, result.Message));
            await LogManualAmenityAccessIfNeededAsync(staffUserId, resident.UserId, amenity.AmenityName, false, result.Message, isManualOverride);
            return result;
        }

        if (!isWithinOperatingHours)
        {
            result.Message = $"Tiện ích đang ngoài giờ hoạt động ({amenity.OpenTime:hh\\:mm} - {amenity.CloseTime:hh\\:mm}).";
            await LogFaceAccessAttemptAsync(resident.UserId, false, confidenceScore, ipAddress, BuildDeviceInfo(deviceInfo, amenity.AmenityName, isManualOverride, result.Message));
            await LogManualAmenityAccessIfNeededAsync(staffUserId, resident.UserId, amenity.AmenityName, false, result.Message, isManualOverride);
            return result;
        }

        if (!amenity.RequiresBooking)
        {
            result.AccessGranted = true;
            result.HasValidBooking = true;
            result.Message = "Xác thực thành công. Cư dân được phép sử dụng tiện ích mở tự do.";

            await LogFaceAccessAttemptAsync(resident.UserId, true, confidenceScore, ipAddress, BuildDeviceInfo(deviceInfo, amenity.AmenityName, isManualOverride, "Tiện ích mở tự do"));
            await LogManualAmenityAccessIfNeededAsync(staffUserId, resident.UserId, amenity.AmenityName, true, result.Message, isManualOverride);
            return result;
        }

        var activeBooking = await _context.AmenityBookings
            .AsNoTracking()
            .Where(b =>
                b.AmenityId == amenity.AmenityId &&
                b.ResidentId == resident.UserId &&
                b.BookingDate == now.Date &&
                b.Status == AmenityBookingStatusHelper.Confirmed &&
                b.StartTime <= now.TimeOfDay &&
                b.EndTime > now.TimeOfDay)
            .OrderBy(b => b.StartTime)
            .FirstOrDefaultAsync();

        if (activeBooking == null)
        {
            result.Message = "Không tìm thấy booking hợp lệ của cư dân tại thời điểm hiện tại.";
            await LogFaceAccessAttemptAsync(resident.UserId, false, confidenceScore, ipAddress, BuildDeviceInfo(deviceInfo, amenity.AmenityName, isManualOverride, result.Message));
            await LogManualAmenityAccessIfNeededAsync(staffUserId, resident.UserId, amenity.AmenityName, false, result.Message, isManualOverride);
            return result;
        }

        result.AccessGranted = true;
        result.HasValidBooking = true;
        result.BookingWindowLabel = $"{activeBooking.BookingDate:dd/MM/yyyy} {activeBooking.StartTime:hh\\:mm} - {activeBooking.EndTime:hh\\:mm}";
        result.Message = "Xác thực thành công. Booking hiện tại hợp lệ cho tiện ích này.";

        await LogFaceAccessAttemptAsync(resident.UserId, true, confidenceScore, ipAddress, BuildDeviceInfo(deviceInfo, amenity.AmenityName, isManualOverride, $"Booking {result.BookingWindowLabel}"));
        await LogManualAmenityAccessIfNeededAsync(staffUserId, resident.UserId, amenity.AmenityName, true, result.Message, isManualOverride);
        return result;
    }

    private async Task<Amenity?> GetAmenityForAccessAsync(int amenityId)
    {
        return await _context.Amenities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AmenityId == amenityId && a.IsActive && !a.IsDeleted);
    }

    private async Task LogFaceAccessAttemptAsync(int residentId, bool isSuccess, double confidenceScore, string? ipAddress, string? deviceInfo)
    {
        await _faceAuthHistoryRepository.AddAsync(new FaceAuthHistory
        {
            ResidentId = residentId,
            AuthTime = DateTime.Now,
            IsSuccess = isSuccess,
            ConfidenceScore = confidenceScore,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo
        });
    }

    private async Task LogManualAmenityAccessIfNeededAsync(int? staffUserId, int residentId, string amenityName, bool success, string message, bool isManualOverride)
    {
        if (!isManualOverride || !staffUserId.HasValue)
        {
            return;
        }

        await _activityLogService.LogCustomAsync(
            action: success ? "ManualAmenityCheckInApproved" : "ManualAmenityCheckInRejected",
            entityName: nameof(User),
            entityId: residentId.ToString(),
            description: $"Nhân viên xác thực thủ công cho tiện ích {amenityName}",
            data: new
            {
                ResidentId = residentId,
                StaffUserId = staffUserId.Value,
                AmenityName = amenityName,
                Success = success,
                Message = message
            });
    }

    private static AmenityFaceAccessResultDto BuildAmenityErrorResult(int amenityId, string message, string? amenityName = null, bool requiresBooking = false)
    {
        return new AmenityFaceAccessResultDto
        {
            CheckedAt = DateTime.Now,
            AmenityId = amenityId,
            AmenityName = amenityName ?? "Không xác định",
            AmenityRequiresBooking = requiresBooking,
            AccessGranted = false,
            Message = message
        };
    }

    private static string BuildDeviceInfo(string? deviceInfo, string amenityName, bool isManualOverride, string note)
    {
        var mode = isManualOverride ? "ManualAmenityCheckIn" : "AmenityFaceScan";
        var normalizedDevice = string.IsNullOrWhiteSpace(deviceInfo) ? "UnknownDevice" : deviceInfo.Trim();
        return $"{mode} | Amenity:{amenityName} | {normalizedDevice} | {note}";
    }

    private static string? ValidateDescriptor(string? faceDescriptor)
    {
        if (string.IsNullOrWhiteSpace(faceDescriptor))
        {
            return "Dữ liệu khuôn mặt không hợp lệ.";
        }

        if (faceDescriptor.Length > 12000)
        {
            return "Dữ liệu khuôn mặt vượt quá kích thước cho phép.";
        }

        try
        {
            using var document = JsonDocument.Parse(faceDescriptor);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return "Dữ liệu khuôn mặt không đúng định dạng.";
            }

            if (document.RootElement.GetArrayLength() != ExpectedDescriptorLength)
            {
                return "Dữ liệu khuôn mặt chưa đủ độ chính xác.";
            }

            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Number)
                {
                    return "Dữ liệu khuôn mặt không đúng định dạng.";
                }
            }
        }
        catch
        {
            return "Không đọc được dữ liệu khuôn mặt.";
        }

        return null;
    }

    private static double[]? ParseDescriptor(string? faceDescriptor)
    {
        if (string.IsNullOrWhiteSpace(faceDescriptor))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(faceDescriptor);
            if (document.RootElement.ValueKind != JsonValueKind.Array || document.RootElement.GetArrayLength() != ExpectedDescriptorLength)
            {
                return null;
            }

            var values = new double[ExpectedDescriptorLength];
            var index = 0;

            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Number)
                {
                    return null;
                }

                values[index++] = item.GetDouble();
            }

            return values;
        }
        catch
        {
            return null;
        }
    }

    private static double CalculateEuclideanDistance(double[] first, double[] second)
    {
        var sum = 0d;
        for (var index = 0; index < ExpectedDescriptorLength; index++)
        {
            var difference = first[index] - second[index];
            sum += difference * difference;
        }

        return Math.Sqrt(sum);
    }

    private static double NormalizeConfidence(double distance)
    {
        return Math.Clamp(1 - distance, 0, 1);
    }

    private static bool IsAmenityOpenNow(Amenity amenity, DateTime currentTime)
    {
        var now = currentTime.TimeOfDay;
        return now >= amenity.OpenTime && now <= amenity.CloseTime;
    }
}
