using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class AmenityService : IAmenityService
{
    private static readonly TimeSpan SlotDuration = TimeSpan.FromHours(1);

    private readonly ApartmentDbContext _context;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IAmenityBookingRepository _amenityBookingRepository;
    private readonly INotificationService _notificationService;

    public AmenityService(
        ApartmentDbContext context,
        IAmenityRepository amenityRepository,
        IAmenityBookingRepository amenityBookingRepository,
        INotificationService notificationService)
    {
        _context = context;
        _amenityRepository = amenityRepository;
        _amenityBookingRepository = amenityBookingRepository;
        _notificationService = notificationService;
    }

    public async Task<IReadOnlyList<Amenity>> GetManagerAmenitiesAsync(string? search, bool? isActive, bool? requiresBooking)
    {
        var query = BaseAmenityQuery();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            query = query.Where(a =>
                a.AmenityName.ToLower().Contains(keyword) ||
                (a.Location != null && a.Location.ToLower().Contains(keyword)) ||
                (a.AmenityType != null && a.AmenityType.TypeName.ToLower().Contains(keyword)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(a => a.IsActive == isActive.Value);
        }

        if (requiresBooking.HasValue)
        {
            query = query.Where(a => a.RequiresBooking == requiresBooking.Value);
        }

        return await query
            .OrderBy(a => a.AmenityName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AmenityType>> GetAmenityTypesAsync()
    {
        return await _context.AmenityTypes
            .Where(t => t.IsActive && !t.IsDeleted)
            .OrderBy(t => t.TypeName)
            .ToListAsync();
    }

    public async Task<Amenity?> GetAmenityByIdAsync(int amenityId)
    {
        return await BaseAmenityQuery()
            .FirstOrDefaultAsync(a => a.AmenityId == amenityId);
    }

    public async Task<(bool Success, string Message, Amenity? Amenity)> SaveAmenityAsync(Amenity amenity)
    {
        amenity.AmenityName = amenity.AmenityName.Trim();
        amenity.Location = string.IsNullOrWhiteSpace(amenity.Location) ? null : amenity.Location.Trim();
        amenity.Description = string.IsNullOrWhiteSpace(amenity.Description) ? null : amenity.Description.Trim();

        var validationMessage = await ValidateAmenityAsync(amenity);
        if (validationMessage != null)
        {
            return (false, validationMessage, null);
        }

        if (!amenity.RequiresBooking)
        {
            amenity.PricePerHour = 0;
            amenity.CancellationDeadlineHours = 0;
        }

        if (amenity.AmenityId == 0)
        {
            amenity.CreatedAt = DateTime.Now;
            amenity.IsDeleted = false;
            await _amenityRepository.AddAsync(amenity);
            return (true, "Đã thêm tiện ích mới.", amenity);
        }

        var existingAmenity = await _context.Amenities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AmenityId == amenity.AmenityId && !a.IsDeleted);

        if (existingAmenity == null)
        {
            return (false, "Không tìm thấy tiện ích cần cập nhật.", null);
        }

        amenity.CreatedAt = existingAmenity.CreatedAt;
        amenity.IsDeleted = existingAmenity.IsDeleted;

        await _amenityRepository.UpdateAsync(amenity);
        return (true, "Đã cập nhật tiện ích.", amenity);
    }

    public async Task<(bool Success, string Message)> DeleteAmenityAsync(int amenityId)
    {
        var amenity = await _context.Amenities.FirstOrDefaultAsync(a => a.AmenityId == amenityId && !a.IsDeleted);
        if (amenity == null)
        {
            return (false, "Không tìm thấy tiện ích.");
        }

        var now = DateTime.Now;
        var hasFutureBookings = await _context.AmenityBookings
            .AnyAsync(b =>
                b.AmenityId == amenityId &&
                b.Status != AmenityBookingStatusHelper.Cancelled &&
                (b.BookingDate > now.Date || (b.BookingDate == now.Date && b.EndTime > now.TimeOfDay)));

        if (hasFutureBookings)
        {
            return (false, "Tiện ích đang có lịch đặt sắp tới, không thể xóa.");
        }

        amenity.IsDeleted = true;
        amenity.IsActive = false;
        await _amenityRepository.UpdateAsync(amenity);

        return (true, "Đã ngừng hiển thị tiện ích.");
    }

    public async Task<IReadOnlyList<Amenity>> GetResidentAmenitiesAsync(string? search = null)
    {
        var query = BaseAmenityQuery()
            .Where(a => a.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            query = query.Where(a =>
                a.AmenityName.ToLower().Contains(keyword) ||
                (a.Location != null && a.Location.ToLower().Contains(keyword)) ||
                (a.AmenityType != null && a.AmenityType.TypeName.ToLower().Contains(keyword)));
        }

        return await query
            .OrderByDescending(a => a.RequiresBooking)
            .ThenBy(a => a.AmenityName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AmenityAvailabilitySlotDto>> GetAvailabilitySlotsAsync(int amenityId, DateTime bookingDate)
    {
        var amenity = await _context.Amenities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AmenityId == amenityId && a.IsActive && !a.IsDeleted);

        if (amenity == null || !amenity.RequiresBooking)
        {
            return [];
        }

        var bookings = await GetBlockingBookingsQuery()
            .Where(b => b.AmenityId == amenityId && b.BookingDate == bookingDate.Date)
            .ToListAsync();

        var slots = new List<AmenityAvailabilitySlotDto>();
        for (var start = amenity.OpenTime; start + SlotDuration <= amenity.CloseTime; start += SlotDuration)
        {
            var end = start + SlotDuration;
            var isAvailable = !bookings.Any(b => IsOverlapping(start, end, b.StartTime, b.EndTime));

            slots.Add(new AmenityAvailabilitySlotDto
            {
                StartTime = start,
                EndTime = end,
                IsAvailable = isAvailable,
                StatusLabel = isAvailable ? "Còn trống" : "Đã được đặt"
            });
        }

        return slots;
    }

    public async Task<(bool Success, string Message, AmenityBooking? Booking)> CreateBookingAsync(
        int residentId,
        int amenityId,
        DateTime bookingDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int participantCount,
        string? notes)
    {
        var resident = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == residentId && u.IsActive && !u.IsDeleted);

        if (resident?.ApartmentId == null)
        {
            return (false, "Bạn chưa được gán căn hộ. Vui lòng liên hệ Ban Quản Lý.", null);
        }

        var amenity = await _context.Amenities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AmenityId == amenityId && a.IsActive && !a.IsDeleted);

        if (amenity == null)
        {
            return (false, "Không tìm thấy tiện ích cần đặt.", null);
        }

        if (!amenity.RequiresBooking)
        {
            return (false, "Tiện ích này đang mở tự do cho cư dân, không cần đặt trước.", null);
        }

        var validationMessage = await ValidateBookingAsync(amenity, residentId, bookingDate, startTime, endTime, participantCount);
        if (validationMessage != null)
        {
            return (false, validationMessage, null);
        }

        var totalHours = decimal.Round((decimal)(endTime - startTime).TotalHours, 2);
        var booking = new AmenityBooking
        {
            AmenityId = amenity.AmenityId,
            ApartmentId = resident.ApartmentId.Value,
            ResidentId = residentId,
            BookingDate = bookingDate.Date,
            StartTime = startTime,
            EndTime = endTime,
            ParticipantCount = participantCount,
            TotalHours = totalHours,
            TotalAmount = decimal.Round(totalHours * (amenity.PricePerHour ?? 0), 2),
            Status = AmenityBookingStatusHelper.Confirmed,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            CreatedAt = DateTime.Now
        };

        await _amenityBookingRepository.AddAsync(booking);
        await NotifyResidentAsync(residentId, amenity, booking, AmenityBookingStatusHelper.Confirmed);

        return (true, "Đặt tiện ích thành công.", booking);
    }

    public async Task<IReadOnlyList<AmenityBooking>> GetResidentBookingsAsync(int residentId, string? status = null)
    {
        var query = _context.AmenityBookings
            .Include(b => b.Amenity)
                .ThenInclude(a => a.AmenityType)
            .Include(b => b.Apartment)
            .Where(b => b.ResidentId == residentId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(b => b.Status == status);
        }

        return await query
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> CancelBookingAsync(int bookingId, int residentId)
    {
        var booking = await _context.AmenityBookings
            .Include(b => b.Amenity)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.ResidentId == residentId);

        if (booking == null)
        {
            return (false, "Không tìm thấy booking cần hủy.");
        }

        if (!string.Equals(booking.Status, AmenityBookingStatusHelper.Confirmed, StringComparison.OrdinalIgnoreCase))
        {
            return (false, "Booking này không còn ở trạng thái có thể hủy.");
        }

        var startDateTime = booking.BookingDate.Date.Add(booking.StartTime);
        var cancelDeadline = startDateTime.AddHours(-booking.Amenity.CancellationDeadlineHours);

        if (DateTime.Now >= cancelDeadline)
        {
            return (false, $"Chỉ có thể hủy trước giờ sử dụng ít nhất {booking.Amenity.CancellationDeadlineHours} giờ.");
        }

        booking.Status = AmenityBookingStatusHelper.Cancelled;
        booking.UpdatedAt = DateTime.Now;
        await _amenityBookingRepository.UpdateAsync(booking);

        await NotifyResidentAsync(booking.ResidentId, booking.Amenity, booking, AmenityBookingStatusHelper.Cancelled);
        return (true, "Đã hủy booking tiện ích.");
    }

    public async Task<bool> ResidentHasApartmentAsync(int residentId)
    {
        return await _context.Users
            .AnyAsync(u => u.UserId == residentId && u.ApartmentId != null && u.IsActive && !u.IsDeleted);
    }

    private IQueryable<Amenity> BaseAmenityQuery()
    {
        return _context.Amenities
            .Include(a => a.AmenityType)
            .Where(a => !a.IsDeleted);
    }

    private IQueryable<AmenityBooking> GetBlockingBookingsQuery()
    {
        return _context.AmenityBookings
            .Where(b => b.Status != AmenityBookingStatusHelper.Cancelled);
    }

    private async Task<string?> ValidateAmenityAsync(Amenity amenity)
    {
        if (string.IsNullOrWhiteSpace(amenity.AmenityName))
        {
            return "Vui lòng nhập tên tiện ích.";
        }

        if (!amenity.AmenityTypeId.HasValue ||
            !await _context.AmenityTypes.AnyAsync(t => t.AmenityTypeId == amenity.AmenityTypeId && !t.IsDeleted))
        {
            return "Loại tiện ích không hợp lệ.";
        }

        if (!amenity.Capacity.HasValue || amenity.Capacity.Value <= 0)
        {
            return "Sức chứa phải lớn hơn 0.";
        }

        if (amenity.OpenTime >= amenity.CloseTime)
        {
            return "Giờ mở cửa phải nhỏ hơn giờ đóng cửa.";
        }

        if (amenity.PricePerHour < 0)
        {
            return "Giá theo giờ không được âm.";
        }

        if (amenity.CancellationDeadlineHours < 0)
        {
            return "Thời hạn hủy phải lớn hơn hoặc bằng 0.";
        }

        var normalizedName = amenity.AmenityName.Trim().ToLower();
        var isDuplicate = await _context.Amenities
            .AnyAsync(a => !a.IsDeleted &&
                           a.AmenityId != amenity.AmenityId &&
                           a.AmenityName.ToLower() == normalizedName);

        if (isDuplicate)
        {
            return "Tên tiện ích đã tồn tại.";
        }

        return null;
    }

    private async Task<string?> ValidateBookingAsync(
        Amenity amenity,
        int residentId,
        DateTime bookingDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int participantCount)
    {
        if (bookingDate.Date < DateTime.Now.Date)
        {
            return "Không thể đặt lịch cho ngày trong quá khứ.";
        }

        if (startTime >= endTime)
        {
            return "Giờ bắt đầu phải nhỏ hơn giờ kết thúc.";
        }

        if (startTime.Minutes != 0 || startTime.Seconds != 0 || endTime.Minutes != 0 || endTime.Seconds != 0)
        {
            return "Chỉ hỗ trợ đặt theo khung giờ tròn.";
        }

        if (startTime < amenity.OpenTime || endTime > amenity.CloseTime)
        {
            return "Khung giờ đã chọn nằm ngoài giờ mở cửa của tiện ích.";
        }

        if (bookingDate.Date == DateTime.Now.Date && bookingDate.Date.Add(startTime) <= DateTime.Now)
        {
            return "Khung giờ bắt đầu phải ở tương lai.";
        }

        if (participantCount <= 0)
        {
            return "Số người tham gia phải lớn hơn 0.";
        }

        if (amenity.Capacity.HasValue && participantCount > amenity.Capacity.Value)
        {
            return $"Số người tham gia không được vượt quá sức chứa {amenity.Capacity.Value}.";
        }

        var overlappingBookingExists = await GetBlockingBookingsQuery()
            .AnyAsync(b =>
                b.AmenityId == amenity.AmenityId &&
                b.BookingDate == bookingDate.Date &&
                IsOverlapping(startTime, endTime, b.StartTime, b.EndTime));

        if (overlappingBookingExists)
        {
            return "Khung giờ này đã có người đặt. Vui lòng chọn slot khác.";
        }

        var residentOverlapExists = await GetBlockingBookingsQuery()
            .AnyAsync(b =>
                b.ResidentId == residentId &&
                b.BookingDate == bookingDate.Date &&
                IsOverlapping(startTime, endTime, b.StartTime, b.EndTime));

        if (residentOverlapExists)
        {
            return "Bạn đang có booking khác trùng thời gian này.";
        }

        return null;
    }

    private async Task NotifyResidentAsync(int residentId, Amenity amenity, AmenityBooking booking, string status)
    {
        var (title, content) = NotificationUtils.CreateAmenityBookingNotification(
            amenity.AmenityName,
            booking.BookingDate,
            booking.StartTime.ToString(@"hh\:mm"),
            booking.EndTime.ToString(@"hh\:mm"),
            status);

        await _notificationService.CreateNotificationAsync(
            residentId,
            title,
            content,
            NotificationType.Amenity,
            ReferenceType.Amenity,
            amenity.AmenityId,
            NotificationPriority.Normal);
    }

    private static bool IsOverlapping(TimeSpan startA, TimeSpan endA, TimeSpan startB, TimeSpan endB)
    {
        return startA < endB && endA > startB;
    }
}
