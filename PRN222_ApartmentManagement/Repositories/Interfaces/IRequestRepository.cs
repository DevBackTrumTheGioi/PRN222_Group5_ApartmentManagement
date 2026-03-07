using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;

public interface IRequestRepository : IGenericRepository<Request>
{
    /// <summary>Lấy danh sách yêu cầu kèm thông tin liên quan</summary>
    Task<IEnumerable<Request>> GetAllWithDetailsAsync();

    /// <summary>Lấy chi tiết một yêu cầu kèm attachments</summary>
    Task<Request?> GetWithDetailsAsync(int requestId);

    /// <summary>Lấy yêu cầu theo cư dân</summary>
    Task<IEnumerable<Request>> GetByResidentIdAsync(int residentId);

    /// <summary>Lấy yêu cầu được giao cho staff</summary>
    Task<IEnumerable<Request>> GetByAssignedToAsync(int staffId);

    /// <summary>Lấy yêu cầu theo trạng thái</summary>
    Task<IEnumerable<Request>> GetByStatusAsync(RequestStatus status);

    /// <summary>Lấy khiếu nại (RequestType = Complaint)</summary>
    Task<IEnumerable<Request>> GetComplaintsAsync();

    /// <summary>Sinh số yêu cầu tự động: REQ-YYYYMM-XXXX</summary>
    Task<string> GenerateRequestNumberAsync();
}

