using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;

/// <summary>
/// Repository interface for ServiceOrder entity
/// </summary>
public interface IServiceOrderRepository : IGenericRepository<ServiceOrder>
{
    /// <summary>
    /// Get service orders by apartment
    /// </summary>
    Task<IEnumerable<ServiceOrder>> GetByApartmentIdAsync(int apartmentId);

    /// <summary>
    /// Get service orders by resident
    /// </summary>
    Task<IEnumerable<ServiceOrder>> GetByResidentIdAsync(int residentId);

    /// <summary>
    /// Get service orders by status
    /// </summary>
    Task<IEnumerable<ServiceOrder>> GetByStatusAsync(ServiceOrderStatus status);

    /// <summary>
    /// Get service orders assigned to a staff member
    /// </summary>
    Task<IEnumerable<ServiceOrder>> GetByAssignedStaffAsync(int staffId);

    /// <summary>
    /// Get service orders by service type
    /// </summary>
    Task<IEnumerable<ServiceOrder>> GetByServiceTypeAsync(int serviceTypeId);

    /// <summary>
    /// Get service orders for a specific date
    /// </summary>
    Task<IEnumerable<ServiceOrder>> GetByRequestedDateAsync(DateTime date);

    /// <summary>
    /// Get pending service orders
    /// </summary>
    Task<IEnumerable<ServiceOrder>> GetPendingOrdersAsync();

    /// <summary>
    /// Generate next order number
    /// </summary>
    Task<string> GenerateOrderNumberAsync();

    /// <summary>
    /// Get service order with all related entities
    /// </summary>
    Task<ServiceOrder?> GetWithDetailsAsync(int serviceOrderId);

    /// <summary>
    /// Get all service orders with related entities loaded (for lists)
    /// </summary>
    Task<IEnumerable<ServiceOrder>> GetAllWithDetailsAsync();
}

