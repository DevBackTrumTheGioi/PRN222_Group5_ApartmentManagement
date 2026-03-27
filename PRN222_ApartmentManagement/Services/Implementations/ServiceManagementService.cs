using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class ServiceManagementService : IServiceManagementService
{
    private static readonly string[] AllowedTimeSlots = ["Morning", "Afternoon", "Evening", "Sáng", "Chiều", "Tối"];

    private readonly ApartmentDbContext _context;
    private readonly IServiceTypeRepository _serviceTypeRepository;
    private readonly IServicePriceRepository _servicePriceRepository;
    private readonly IServiceOrderRepository _serviceOrderRepository;
    private readonly IResidentApartmentAccessService _residentApartmentAccessService;

    public ServiceManagementService(
        ApartmentDbContext context,
        IServiceTypeRepository serviceTypeRepository,
        IServicePriceRepository servicePriceRepository,
        IServiceOrderRepository serviceOrderRepository,
        IResidentApartmentAccessService residentApartmentAccessService)
    {
        _context = context;
        _serviceTypeRepository = serviceTypeRepository;
        _servicePriceRepository = servicePriceRepository;
        _serviceOrderRepository = serviceOrderRepository;
        _residentApartmentAccessService = residentApartmentAccessService;
    }

    public async Task<IReadOnlyList<ServiceType>> GetManagerServiceTypesAsync(string? search, bool? isActive)
    {
        var query = BaseServiceTypeQuery();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            query = query.Where(st =>
                st.ServiceTypeName.ToLower().Contains(keyword) ||
                (st.Description != null && st.Description.ToLower().Contains(keyword)) ||
                (st.MeasurementUnit != null && st.MeasurementUnit.ToLower().Contains(keyword)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(st => st.IsActive == isActive.Value);
        }

        return await query
            .OrderByDescending(st => st.IsActive)
            .ThenBy(st => st.ServiceTypeName)
            .ToListAsync();
    }

    public async Task<ServiceType?> GetServiceTypeWithPricesAsync(int serviceTypeId)
    {
        return await BaseServiceTypeQuery()
            .FirstOrDefaultAsync(st => st.ServiceTypeId == serviceTypeId);
    }

    public async Task<(bool Success, string Message)> ToggleServiceTypeStatusAsync(int serviceTypeId)
    {
        var serviceType = await _context.ServiceTypes
            .FirstOrDefaultAsync(st => st.ServiceTypeId == serviceTypeId && !st.IsDeleted);

        if (serviceType == null)
        {
            return (false, "Không tìm thấy loại dịch vụ.");
        }

        serviceType.IsActive = !serviceType.IsActive;
        serviceType.UpdatedAt = DateTime.Now;
        await _serviceTypeRepository.UpdateAsync(serviceType);

        return (true, serviceType.IsActive ? "Đã kích hoạt dịch vụ." : "Đã tạm ngưng dịch vụ.");
    }

    public async Task<(bool Success, string Message, ServiceType? ServiceType)> SaveServiceTypeAsync(
        ServiceType serviceType,
        decimal unitPrice,
        string? priceDescription)
    {
        serviceType.ServiceTypeName = serviceType.ServiceTypeName.Trim();
        serviceType.Description = NormalizeText(serviceType.Description, 500);
        serviceType.MeasurementUnit = NormalizeText(serviceType.MeasurementUnit, 50);
        priceDescription = NormalizeText(priceDescription, 500);

        if (string.IsNullOrWhiteSpace(serviceType.ServiceTypeName))
        {
            return (false, "Vui lòng nhập tên dịch vụ.", null);
        }

        if (unitPrice < 0)
        {
            return (false, "Đơn giá không hợp lệ.", null);
        }

        var duplicateExists = await _context.ServiceTypes
            .AnyAsync(st =>
                !st.IsDeleted &&
                st.ServiceTypeId != serviceType.ServiceTypeId &&
                st.ServiceTypeName.ToLower() == serviceType.ServiceTypeName.ToLower());

        if (duplicateExists)
        {
            return (false, "Tên dịch vụ đã tồn tại.", null);
        }

        if (serviceType.ServiceTypeId == 0)
        {
            serviceType.CreatedAt = DateTime.Now;
            serviceType.UpdatedAt = null;
            serviceType.IsDeleted = false;
            await _serviceTypeRepository.AddAsync(serviceType);
        }
        else
        {
            var existing = await _context.ServiceTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(st => st.ServiceTypeId == serviceType.ServiceTypeId && !st.IsDeleted);

            if (existing == null)
            {
                return (false, "Không tìm thấy loại dịch vụ cần cập nhật.", null);
            }

            serviceType.CreatedAt = existing.CreatedAt;
            serviceType.IsDeleted = existing.IsDeleted;
            serviceType.UpdatedAt = DateTime.Now;
            await _serviceTypeRepository.UpdateAsync(serviceType);
        }

        await UpsertCurrentPriceAsync(serviceType.ServiceTypeId, unitPrice, priceDescription);

        var reloaded = await GetServiceTypeWithPricesAsync(serviceType.ServiceTypeId);
        var message = serviceType.ServiceTypeId == 0
            ? "Đã thêm loại dịch vụ mới."
            : "Đã cập nhật loại dịch vụ.";

        return (true, message, reloaded);
    }

    public async Task<IReadOnlyList<ServiceType>> GetResidentActiveServiceTypesAsync(string? search)
    {
        var today = DateTime.Now.Date;
        var query = BaseServiceTypeQuery()
            .Where(st => st.IsActive && st.ServicePrices.Any(sp =>
                sp.EffectiveFrom <= today &&
                (!sp.EffectiveTo.HasValue || sp.EffectiveTo.Value >= today)));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            query = query.Where(st =>
                st.ServiceTypeName.ToLower().Contains(keyword) ||
                (st.Description != null && st.Description.ToLower().Contains(keyword)) ||
                (st.MeasurementUnit != null && st.MeasurementUnit.ToLower().Contains(keyword)));
        }

        return await query
            .OrderBy(st => st.ServiceTypeName)
            .ToListAsync();
    }

    public async Task<(bool Success, string Message, ServiceOrder? Order)> CreateOrderAsync(
        int residentId,
        int apartmentId,
        int serviceTypeId,
        DateTime requestedDate,
        string? requestedTimeSlot,
        string? description)
    {
        var resident = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == residentId && u.IsActive && !u.IsDeleted && u.Role == UserRole.Resident);

        if (resident == null)
        {
            return (false, "Bạn chưa được gán căn hộ. Vui lòng liên hệ Ban Quản lý.", null);
        }

        if (!await _residentApartmentAccessService.IsResidentInApartmentAsync(residentId, apartmentId))
        {
            return (false, "Căn hộ đã chọn không thuộc quyền sử dụng hiện tại của bạn.", null);
        }

        if (requestedDate.Date < DateTime.Now.Date)
        {
            return (false, "Ngày sử dụng phải từ hôm nay trở đi.", null);
        }

        if (string.IsNullOrWhiteSpace(requestedTimeSlot))
        {
            return (false, "Vui lòng chọn khung giờ yêu cầu.", null);
        }

        if (!AllowedTimeSlots.Contains(requestedTimeSlot.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            return (false, "Khung giờ không hợp lệ.", null);
        }

        var serviceType = await _context.ServiceTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(st => st.ServiceTypeId == serviceTypeId && st.IsActive && !st.IsDeleted);

        if (serviceType == null)
        {
            return (false, "Dịch vụ không tồn tại hoặc đã tạm ngưng.", null);
        }

        var effectivePrice = await GetEffectivePriceAsync(serviceTypeId, DateTime.Now.Date);
        if (effectivePrice == null)
        {
            return (false, "Dịch vụ này chưa có bảng giá hiện hành.", null);
        }

        var order = new ServiceOrder
        {
            OrderNumber = await _serviceOrderRepository.GenerateOrderNumberAsync(),
            ApartmentId = apartmentId,
            ResidentId = residentId,
            ServiceTypeId = serviceTypeId,
            RequestedDate = requestedDate.Date,
            RequestedTimeSlot = NormalizeTimeSlot(requestedTimeSlot),
            Description = NormalizeText(description, 2000),
            Status = ServiceOrderStatus.Pending,
            EstimatedPrice = effectivePrice.UnitPrice,
            CreatedAt = DateTime.Now,
            UpdatedAt = null,
            AdditionalCharges = 0
        };

        await _serviceOrderRepository.AddAsync(order);

        var reloaded = await GetServiceOrderAsync(order.ServiceOrderId);
        return (true, $"Đã tạo đơn dịch vụ {order.OrderNumber}.", reloaded);
    }

    public async Task<IReadOnlyList<ServiceOrder>> GetResidentOrdersAsync(int residentId, ServiceOrderStatus? status)
    {
        var query = BaseServiceOrderQuery()
            .Where(so => so.ResidentId == residentId);

        if (status.HasValue)
        {
            query = query.Where(so => so.Status == status.Value);
        }

        return await query
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync();
    }

    public async Task<ServiceOrder?> GetServiceOrderAsync(int serviceOrderId)
    {
        return await BaseServiceOrderQuery()
            .FirstOrDefaultAsync(so => so.ServiceOrderId == serviceOrderId);
    }

    public async Task<(bool Success, string Message)> CancelOrderAsync(int serviceOrderId, int residentId, string? cancelReason)
    {
        var order = await _context.ServiceOrders
            .FirstOrDefaultAsync(so => so.ServiceOrderId == serviceOrderId && so.ResidentId == residentId);

        if (order == null)
        {
            return (false, "Không tìm thấy đơn dịch vụ.");
        }

        if (order.Status != ServiceOrderStatus.Pending)
        {
            return (false, "Chỉ có thể hủy đơn khi đang ở trạng thái chờ xử lý.");
        }

        order.Status = ServiceOrderStatus.Cancelled;
        order.CancelledAt = DateTime.Now;
        order.CancelReason = NormalizeText(cancelReason, 500) ?? "Cư dân hủy đơn";
        order.UpdatedAt = DateTime.Now;

        await _serviceOrderRepository.UpdateAsync(order);
        return (true, "Đã hủy đơn dịch vụ.");
    }

    public async Task<(bool Success, string Message)> SubmitReviewAsync(int serviceOrderId, int residentId, int rating, string? reviewComment)
    {
        var order = await _context.ServiceOrders
            .FirstOrDefaultAsync(so => so.ServiceOrderId == serviceOrderId && so.ResidentId == residentId);

        if (order == null)
        {
            return (false, "Không tìm thấy đơn dịch vụ.");
        }

        if (order.Status != ServiceOrderStatus.Completed)
        {
            return (false, "Chỉ có thể đánh giá đơn đã hoàn thành.");
        }

        if (order.Rating.HasValue)
        {
            return (false, "Đơn này đã được đánh giá trước đó.");
        }

        if (rating < 1 || rating > 5)
        {
            return (false, "Số sao đánh giá không hợp lệ.");
        }

        order.Rating = rating;
        order.ReviewComment = NormalizeText(reviewComment, 500);
        order.ReviewedAt = DateTime.Now;
        order.UpdatedAt = DateTime.Now;

        await _serviceOrderRepository.UpdateAsync(order);
        return (true, "Cảm ơn bạn đã đánh giá dịch vụ.");
    }

    public async Task<IReadOnlyList<ServiceOrder>> GetManagerOrdersAsync(ServiceOrderStatus? status, int? serviceTypeId, string? search)
    {
        var query = BaseServiceOrderQuery();

        if (status.HasValue)
        {
            query = query.Where(so => so.Status == status.Value);
        }

        if (serviceTypeId.HasValue)
        {
            query = query.Where(so => so.ServiceTypeId == serviceTypeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            query = query.Where(so =>
                so.OrderNumber.ToLower().Contains(keyword) ||
                so.ServiceType.ServiceTypeName.ToLower().Contains(keyword) ||
                so.Resident.FullName.ToLower().Contains(keyword) ||
                so.Apartment.ApartmentNumber.ToLower().Contains(keyword) ||
                (so.Description != null && so.Description.ToLower().Contains(keyword)));
        }

        return await query
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<User>> GetAvailableStaffAsync()
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.BQL_Staff && u.IsActive && !u.IsDeleted)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> AssignOrderAsync(int serviceOrderId, int staffId)
    {
        var order = await _context.ServiceOrders
            .FirstOrDefaultAsync(so => so.ServiceOrderId == serviceOrderId);

        if (order == null)
        {
            return (false, "Không tìm thấy đơn dịch vụ.");
        }

        if (IsClosed(order.Status))
        {
            return (false, "Đơn đã đóng, không thể phân công.");
        }

        var staff = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == staffId && u.Role == UserRole.BQL_Staff && u.IsActive && !u.IsDeleted);

        if (staff == null)
        {
            return (false, "Nhân viên được chọn không hợp lệ.");
        }

        order.AssignedTo = staffId;
        order.AssignedAt = DateTime.Now;
        order.UpdatedAt = DateTime.Now;

        await _serviceOrderRepository.UpdateAsync(order);
        return (true, "Đã phân công nhân viên xử lý đơn.");
    }

    public async Task<IReadOnlyList<ServiceOrder>> GetAssignedOrdersAsync(int staffId, ServiceOrderStatus? status, string? search)
    {
        var query = BaseServiceOrderQuery()
            .Where(so => so.AssignedTo == staffId);

        if (status.HasValue)
        {
            query = query.Where(so => so.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            query = query.Where(so =>
                so.OrderNumber.ToLower().Contains(keyword) ||
                so.ServiceType.ServiceTypeName.ToLower().Contains(keyword) ||
                so.Resident.FullName.ToLower().Contains(keyword) ||
                so.Apartment.ApartmentNumber.ToLower().Contains(keyword) ||
                (so.Description != null && so.Description.ToLower().Contains(keyword)));
        }

        return await query
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> UpdateAssignedOrderAsync(
        int serviceOrderId,
        int staffId,
        ServiceOrderStatus newStatus,
        decimal? actualPrice,
        decimal? additionalCharges,
        string? chargeNotes,
        string? completionNotes)
    {
        var order = await _context.ServiceOrders
            .FirstOrDefaultAsync(so => so.ServiceOrderId == serviceOrderId);

        if (order == null)
        {
            return (false, "Không tìm thấy đơn dịch vụ.");
        }

        if (order.AssignedTo != staffId)
        {
            return (false, "Bạn không được phép cập nhật đơn này.");
        }

        if (!IsAllowedTransition(order.Status, newStatus))
        {
            return (false, "Chuyển trạng thái không hợp lệ.");
        }

        if (actualPrice.HasValue && actualPrice.Value < 0)
        {
            return (false, "Chi phí thực tế không hợp lệ.");
        }

        if (additionalCharges.HasValue && additionalCharges.Value < 0)
        {
            return (false, "Chi phí phát sinh không hợp lệ.");
        }

        order.AdditionalCharges = additionalCharges ?? order.AdditionalCharges;
        order.ChargeNotes = NormalizeText(chargeNotes, 500);
        order.CompletionNotes = NormalizeText(completionNotes, 2000);

        if (actualPrice.HasValue)
        {
            order.ActualPrice = actualPrice.Value;
        }

        order.Status = newStatus;
        order.UpdatedAt = DateTime.Now;

        if (newStatus == ServiceOrderStatus.Completed)
        {
            order.CompletedAt = DateTime.Now;
            order.CompletedBy = staffId;
            order.ActualPrice ??= (order.EstimatedPrice ?? 0) + order.AdditionalCharges;
        }

        await _serviceOrderRepository.UpdateAsync(order);

        var message = newStatus switch
        {
            ServiceOrderStatus.Confirmed => "Đã xác nhận tiếp nhận đơn.",
            ServiceOrderStatus.InProgress => "Đã chuyển đơn sang trạng thái đang thực hiện.",
            ServiceOrderStatus.Completed => "Đã hoàn thành đơn dịch vụ.",
            _ => "Đã cập nhật đơn dịch vụ."
        };

        return (true, message);
    }

    private IQueryable<ServiceType> BaseServiceTypeQuery()
    {
        return _context.ServiceTypes
            .Include(st => st.ServicePrices)
            .Include(st => st.ServiceOrders)
            .Where(st => !st.IsDeleted);
    }

    private IQueryable<ServiceOrder> BaseServiceOrderQuery()
    {
        return _context.ServiceOrders
            .Include(so => so.ServiceType)
                .ThenInclude(st => st.ServicePrices)
            .Include(so => so.Apartment)
            .Include(so => so.Resident)
            .Include(so => so.AssignedStaff)
            .Include(so => so.CompletedByUser)
            .Include(so => so.Invoice);
    }

    private async Task<ServicePrice?> GetEffectivePriceAsync(int serviceTypeId, DateTime referenceDate)
    {
        return await _context.ServicePrices
            .Where(sp =>
                sp.ServiceTypeId == serviceTypeId &&
                sp.EffectiveFrom <= referenceDate &&
                (!sp.EffectiveTo.HasValue || sp.EffectiveTo.Value >= referenceDate))
            .OrderByDescending(sp => sp.EffectiveFrom)
            .ThenByDescending(sp => sp.ServicePriceId)
            .FirstOrDefaultAsync();
    }

    private async Task UpsertCurrentPriceAsync(int serviceTypeId, decimal unitPrice, string? priceDescription)
    {
        var today = DateTime.Now.Date;
        var currentPrice = await GetEffectivePriceAsync(serviceTypeId, today);

        if (currentPrice != null)
        {
            var isSameValue = currentPrice.UnitPrice == unitPrice &&
                              string.Equals(currentPrice.Description ?? string.Empty, priceDescription ?? string.Empty, StringComparison.Ordinal);

            if (isSameValue)
            {
                return;
            }

            if (currentPrice.EffectiveFrom.Date == today)
            {
                currentPrice.UnitPrice = unitPrice;
                currentPrice.Description = priceDescription;
                await _servicePriceRepository.UpdateAsync(currentPrice);
                return;
            }

            currentPrice.EffectiveTo = today.AddDays(-1);
            await _servicePriceRepository.UpdateAsync(currentPrice);
        }

        var newPrice = new ServicePrice
        {
            ServiceTypeId = serviceTypeId,
            UnitPrice = unitPrice,
            EffectiveFrom = today,
            EffectiveTo = null,
            Description = priceDescription,
            CreatedAt = DateTime.Now
        };

        await _servicePriceRepository.AddAsync(newPrice);
    }

    private static bool IsClosed(ServiceOrderStatus status)
    {
        return status is ServiceOrderStatus.Completed or ServiceOrderStatus.Cancelled or ServiceOrderStatus.Rejected;
    }

    private static bool IsAllowedTransition(ServiceOrderStatus currentStatus, ServiceOrderStatus newStatus)
    {
        return currentStatus switch
        {
            ServiceOrderStatus.Pending => newStatus == ServiceOrderStatus.Confirmed,
            ServiceOrderStatus.Confirmed => newStatus == ServiceOrderStatus.InProgress,
            ServiceOrderStatus.InProgress => newStatus == ServiceOrderStatus.Completed,
            _ => false
        };
    }

    private static string? NormalizeTimeSlot(string? requestedTimeSlot)
    {
        if (string.IsNullOrWhiteSpace(requestedTimeSlot))
        {
            return null;
        }

        var normalized = requestedTimeSlot.Trim();
        return normalized.Length > 50 ? normalized[..50] : normalized;
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
}
