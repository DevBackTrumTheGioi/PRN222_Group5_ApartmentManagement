using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class VisitorManagementService : IVisitorManagementService
{
    private readonly ApartmentDbContext _context;
    private readonly IVisitorRepository _visitorRepository;
    private readonly IResidentApartmentAccessService _residentApartmentAccessService;

    public VisitorManagementService(
        ApartmentDbContext context,
        IVisitorRepository visitorRepository,
        IResidentApartmentAccessService residentApartmentAccessService)
    {
        _context = context;
        _visitorRepository = visitorRepository;
        _residentApartmentAccessService = residentApartmentAccessService;
    }

    public async Task<(bool Success, string Message, Visitor? Visitor)> CreateResidentVisitorAsync(
        int residentId,
        int apartmentId,
        string visitorName,
        string phoneNumber,
        string? identityCard,
        DateTime visitDate,
        string? notes)
    {
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
            return (false, "Bạn chưa được gán căn hộ. Vui lòng liên hệ Ban Quản lý.", null);
        }

        visitorName = visitorName.Trim();
        phoneNumber = NormalizePhoneNumber(phoneNumber);
        identityCard = NormalizeIdentityCard(identityCard);
        notes = NormalizeText(notes, 500);

        if (string.IsNullOrWhiteSpace(visitorName))
        {
            return (false, "Vui lòng nhập tên khách.", null);
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return (false, "Vui lòng nhập số điện thoại.", null);
        }

        if (!IsValidPhoneNumber(phoneNumber))
        {
            return (false, "Số điện thoại không hợp lệ. Vui lòng nhập 9 đến 12 chữ số.", null);
        }

        if (!string.IsNullOrWhiteSpace(identityCard) && !IsValidIdentityCard(identityCard))
        {
            return (false, "CCCD/CMND không hợp lệ. Vui lòng chỉ nhập chữ và số.", null);
        }

        if (!await _residentApartmentAccessService.IsResidentInApartmentAsync(residentId, apartmentId))
        {
            return (false, "Căn hộ đã chọn không thuộc quyền sử dụng hiện tại của bạn.", null);
        }

        if (visitDate.Date < DateTime.Now.Date)
        {
            return (false, "Ngày khách đến không được ở quá khứ.", null);
        }

        var normalizedVisitDate = visitDate.Date;
        var hasActiveDuplicate = await _context.Visitors.AnyAsync(v =>
            v.ApartmentId == apartmentId &&
            v.VisitDate == normalizedVisitDate &&
            (v.Status == VisitorStatus.Pending || v.Status == VisitorStatus.CheckedIn) &&
            (v.PhoneNumber == phoneNumber ||
             (!string.IsNullOrWhiteSpace(identityCard) && v.IdentityCard == identityCard)));

        if (hasActiveDuplicate)
        {
            return (false, "Khách này đã có đăng ký đang chờ xử lý hoặc đang check-in trong cùng ngày.", null);
        }

        var visitor = new Visitor
        {
            VisitorName = visitorName,
            PhoneNumber = phoneNumber,
            IdentityCard = identityCard,
            ApartmentId = apartmentId,
            RegisteredBy = residentId,
            VisitDate = normalizedVisitDate,
            Status = VisitorStatus.Pending,
            Notes = notes,
            CreatedAt = DateTime.Now
        };

        await _visitorRepository.AddAsync(visitor);

        var createdVisitor = await BaseVisitorQuery()
            .FirstOrDefaultAsync(v => v.VisitorId == visitor.VisitorId);

        return (true, "Đã đăng ký khách thành công.", createdVisitor);
    }

    public async Task<IReadOnlyList<Visitor>> GetResidentVisitorsAsync(int residentId, VisitorStatus? status, DateTime? visitDate)
    {
        await AutoCloseExpiredPendingVisitorsAsync();

        var query = BaseVisitorQuery()
            .Where(v => v.RegisteredBy == residentId);

        if (status.HasValue)
        {
            query = query.Where(v => v.Status == status.Value);
        }

        if (visitDate.HasValue)
        {
            query = query.Where(v => v.VisitDate == visitDate.Value.Date);
        }

        return await query
            .OrderByDescending(v => v.VisitDate)
            .ThenByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> CancelResidentVisitorAsync(int visitorId, int residentId)
    {
        var visitor = await _context.Visitors
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId && v.RegisteredBy == residentId);

        if (visitor == null)
        {
            return (false, "Không tìm thấy đăng ký khách.");
        }

        if (visitor.Status != VisitorStatus.Pending)
        {
            return (false, "Chỉ có thể hủy đăng ký khi khách đang ở trạng thái chờ đến.");
        }

        visitor.Status = VisitorStatus.Cancelled;
        await _visitorRepository.UpdateAsync(visitor);

        return (true, "Đã hủy đăng ký khách.");
    }

    public async Task<IReadOnlyList<Visitor>> GetStaffVisitorsAsync(VisitorStatus? status, DateTime? visitDate, string? search)
    {
        await AutoCloseExpiredPendingVisitorsAsync();

        var query = BaseVisitorQuery();
        var hasExplicitFilter = status.HasValue || visitDate.HasValue || !string.IsNullOrWhiteSpace(search);

        if (!hasExplicitFilter)
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            query = query.Where(v => v.VisitDate == today || v.VisitDate == tomorrow);
        }

        if (status.HasValue)
        {
            query = query.Where(v => v.Status == status.Value);
        }

        if (visitDate.HasValue)
        {
            query = query.Where(v => v.VisitDate == visitDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();
            query = query.Where(v =>
                v.VisitorName.Contains(normalizedSearch) ||
                v.PhoneNumber.Contains(normalizedSearch) ||
                (v.IdentityCard != null && v.IdentityCard.Contains(normalizedSearch)) ||
                v.Apartment.ApartmentNumber.Contains(normalizedSearch) ||
                v.RegisteredByUser.FullName.Contains(normalizedSearch));
        }

        return await query
            .OrderBy(v => v.Status == VisitorStatus.Pending ? 0 : 1)
            .ThenBy(v => Math.Abs(EF.Functions.DateDiffDay(v.VisitDate, DateTime.Now.Date)))
            .ThenByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<Visitor?> GetVisitorByIdAsync(int visitorId)
    {
        return await BaseVisitorQuery()
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);
    }

    public async Task<(bool Success, string Message)> CheckInVisitorAsync(int visitorId)
    {
        var visitor = await _context.Visitors
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);

        if (visitor == null)
        {
            return (false, "Không tìm thấy đăng ký khách.");
        }

        if (visitor.Status != VisitorStatus.Pending)
        {
            return (false, "Chỉ có thể check-in khách đang ở trạng thái chờ đến.");
        }

        if (visitor.CheckInTime.HasValue || visitor.CheckOutTime.HasValue)
        {
            return (false, "Dữ liệu check-in/check-out hiện tại không hợp lệ.");
        }

        if (visitor.VisitDate.Date != DateTime.Now.Date)
        {
            return (false, "Chỉ ưu tiên check-in cho khách có ngày đến là hôm nay.");
        }

        visitor.Status = VisitorStatus.CheckedIn;
        visitor.CheckInTime = DateTime.Now;
        await _visitorRepository.UpdateAsync(visitor);

        return (true, "Đã ghi nhận khách đến.");
    }

    public async Task<(bool Success, string Message)> CheckOutVisitorAsync(int visitorId)
    {
        var visitor = await _context.Visitors
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);

        if (visitor == null)
        {
            return (false, "Không tìm thấy đăng ký khách.");
        }

        if (visitor.Status != VisitorStatus.CheckedIn)
        {
            return (false, "Chỉ có thể check-out khách đang ở trạng thái đã check-in.");
        }

        if (!visitor.CheckInTime.HasValue)
        {
            return (false, "Không thể check-out khi chưa có thời điểm check-in.");
        }

        if (visitor.CheckOutTime.HasValue)
        {
            return (false, "Khách này đã có thời điểm check-out trước đó.");
        }

        var now = DateTime.Now;
        if (now < visitor.CheckInTime.Value)
        {
            return (false, "Không thể check-out trước thời điểm check-in.");
        }

        visitor.Status = VisitorStatus.CheckedOut;
        visitor.CheckOutTime = now;
        await _visitorRepository.UpdateAsync(visitor);

        return (true, "Đã xác nhận khách rời đi.");
    }

    public async Task<(bool Success, string Message)> RejectVisitorAsync(int visitorId)
    {
        var visitor = await _context.Visitors
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);

        if (visitor == null)
        {
            return (false, "Không tìm thấy đăng ký khách.");
        }

        if (visitor.Status != VisitorStatus.Pending)
        {
            return (false, "Chỉ có thể từ chối đăng ký khi khách đang ở trạng thái chờ đến.");
        }

        visitor.Status = VisitorStatus.Rejected;
        await _visitorRepository.UpdateAsync(visitor);

        return (true, "Đã từ chối đăng ký khách.");
    }

    private IQueryable<Visitor> BaseVisitorQuery()
    {
        return _context.Visitors
            .Include(v => v.Apartment)
            .Include(v => v.RegisteredByUser);
    }

    private async Task AutoCloseExpiredPendingVisitorsAsync()
    {
        var today = DateTime.Now.Date;
        var expiredVisitors = await _context.Visitors
            .Where(v => v.Status == VisitorStatus.Pending && v.VisitDate < today)
            .ToListAsync();

        if (expiredVisitors.Count == 0)
        {
            return;
        }

        foreach (var visitor in expiredVisitors)
        {
            visitor.Status = VisitorStatus.Cancelled;
            visitor.Notes = AppendAutoCloseNote(visitor.Notes);
        }

        await _context.SaveChangesAsync();
    }

    private static string? NormalizeText(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        return normalized.Length > maxLength ? normalized[..maxLength] : normalized;
    }

    private static string NormalizePhoneNumber(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }

    private static string? NormalizeIdentityCard(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var compact = new string(value.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        return compact.Length > 20 ? compact[..20] : compact;
    }

    private static bool IsValidPhoneNumber(string value)
    {
        return value.Length is >= 9 and <= 12 && value.All(char.IsDigit);
    }

    private static bool IsValidIdentityCard(string value)
    {
        return value.Length is >= 9 and <= 20 && value.All(char.IsLetterOrDigit);
    }

    private static string AppendAutoCloseNote(string? existingNotes)
    {
        const string systemNote = "Tự động đóng vì đã quá ngày khách đến.";

        if (string.IsNullOrWhiteSpace(existingNotes))
        {
            return systemNote;
        }

        if (existingNotes.Contains(systemNote, StringComparison.OrdinalIgnoreCase))
        {
            return existingNotes;
        }

        var combined = $"{existingNotes.Trim()} | {systemNote}";
        return combined.Length > 500 ? combined[..500] : combined;
    }
}
