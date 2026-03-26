using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IVisitorManagementService
{
    Task<(bool Success, string Message, Visitor? Visitor)> CreateResidentVisitorAsync(
        int residentId,
        string visitorName,
        string phoneNumber,
        string? identityCard,
        DateTime visitDate,
        string? notes);

    Task<IReadOnlyList<Visitor>> GetResidentVisitorsAsync(int residentId, VisitorStatus? status, DateTime? visitDate);
    Task<(bool Success, string Message)> CancelResidentVisitorAsync(int visitorId, int residentId);

    Task<IReadOnlyList<Visitor>> GetStaffVisitorsAsync(VisitorStatus? status, DateTime? visitDate, string? search);
    Task<Visitor?> GetVisitorByIdAsync(int visitorId);
    Task<(bool Success, string Message)> CheckInVisitorAsync(int visitorId);
    Task<(bool Success, string Message)> CheckOutVisitorAsync(int visitorId);
    Task<(bool Success, string Message)> RejectVisitorAsync(int visitorId);
}
