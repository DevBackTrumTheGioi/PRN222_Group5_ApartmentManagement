using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class FaceAuthService : IFaceAuthService
{
    private const int ExpectedDescriptorLength = 128;

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
}
