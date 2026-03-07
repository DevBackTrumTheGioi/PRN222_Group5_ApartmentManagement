using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IRequestService
{
    // Resident
    Task<IEnumerable<Request>> GetMyRequestsAsync(int residentId);
    Task<Request?> GetRequestDetailAsync(int requestId);
    Task<Request> CreateRequestAsync(Request request, List<IFormFile>? attachments);

    // Manager
    Task<IEnumerable<Request>> GetAllRequestsAsync();
    Task AssignRequestAsync(int requestId, int staffId);

    // Staff
    Task<IEnumerable<Request>> GetAssignedRequestsAsync(int staffId);
    Task UpdateStatusAsync(int requestId, RequestStatus status);

    // BQT
    Task<IEnumerable<Request>> GetComplaintsAsync();
}