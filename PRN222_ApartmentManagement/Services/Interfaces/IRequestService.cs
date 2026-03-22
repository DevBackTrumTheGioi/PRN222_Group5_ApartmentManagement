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
    Task<IEnumerable<Request>> GetAllRequestsAsync(RequestStatus? status, RequestType? type, RequestPriority? priority, string? search);
    Task AssignRequestAsync(int requestId, int staffId);
    Task UpdatePriorityAsync(int requestId, RequestPriority priority);

    // Staff
    Task<IEnumerable<Request>> GetAssignedRequestsAsync(int staffId);
    Task UpdateStatusAsync(int requestId, RequestStatus status);
    Task AddCommentAsync(int requestId, int authorId, string content);

    // Escalation
    Task EscalateAsync(int requestId, int escalatedToManagerId, string reason);
    Task<IEnumerable<Request>> GetEscalatedRequestsAsync();

    // BQT
    Task<IEnumerable<Request>> GetComplaintsAsync();
    Task ForwardComplaintAsync(int requestId, int managerId, string reason);

    // BQL — khiếu nại được forward từ BQT
    Task<IEnumerable<Request>> GetForwardedComplaintsAsync(int managerId);
}