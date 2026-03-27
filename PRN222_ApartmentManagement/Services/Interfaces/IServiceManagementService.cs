using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IServiceManagementService
{
    Task<IReadOnlyList<ServiceType>> GetManagerServiceTypesAsync(string? search, bool? isActive);
    Task<ServiceType?> GetServiceTypeWithPricesAsync(int serviceTypeId);
    Task<(bool Success, string Message)> ToggleServiceTypeStatusAsync(int serviceTypeId);
    Task<(bool Success, string Message, ServiceType? ServiceType)> SaveServiceTypeAsync(
        ServiceType serviceType,
        decimal unitPrice,
        string? priceDescription);

    Task<IReadOnlyList<ServiceType>> GetResidentActiveServiceTypesAsync(string? search);
    Task<(bool Success, string Message, ServiceOrder? Order)> CreateOrderAsync(
        int residentId,
        int apartmentId,
        int serviceTypeId,
        DateTime requestedDate,
        string? requestedTimeSlot,
        string? description);
    Task<IReadOnlyList<ServiceOrder>> GetResidentOrdersAsync(int residentId, ServiceOrderStatus? status);
    Task<ServiceOrder?> GetServiceOrderAsync(int serviceOrderId);
    Task<(bool Success, string Message)> CancelOrderAsync(int serviceOrderId, int residentId, string? cancelReason);
    Task<(bool Success, string Message)> SubmitReviewAsync(int serviceOrderId, int residentId, int rating, string? reviewComment);

    Task<IReadOnlyList<ServiceOrder>> GetManagerOrdersAsync(ServiceOrderStatus? status, int? serviceTypeId, string? search);
    Task<IReadOnlyList<User>> GetAvailableStaffAsync();
    Task<(bool Success, string Message)> AssignOrderAsync(int serviceOrderId, int staffId);

    Task<IReadOnlyList<ServiceOrder>> GetAssignedOrdersAsync(int staffId, ServiceOrderStatus? status, string? search);
    Task<(bool Success, string Message)> UpdateAssignedOrderAsync(
        int serviceOrderId,
        int staffId,
        ServiceOrderStatus newStatus,
        decimal? actualPrice,
        decimal? additionalCharges,
        string? chargeNotes,
        string? completionNotes);
}
