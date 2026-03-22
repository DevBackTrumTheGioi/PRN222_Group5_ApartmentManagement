using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _requestRepository;
    private readonly IRequestAttachmentRepository _attachmentRepository;
    private readonly IWebHostEnvironment _environment;
    private readonly ApartmentDbContext _context;

    public RequestService(
        IRequestRepository requestRepository,
        IRequestAttachmentRepository attachmentRepository,
        IWebHostEnvironment environment,
        ApartmentDbContext context)
    {
        _requestRepository = requestRepository;
        _attachmentRepository = attachmentRepository;
        _environment = environment;
        _context = context;
    }

    public async Task<IEnumerable<Request>> GetMyRequestsAsync(int residentId)
        => SortByUrgency(await _requestRepository.GetByResidentIdAsync(residentId));

    public async Task<Request?> GetRequestDetailAsync(int requestId)
        => await _requestRepository.GetWithDetailsAsync(requestId);

    public async Task<IEnumerable<Request>> GetAllRequestsAsync()
    {
        var all = await _requestRepository.GetAllWithDetailsAsync();
        return SortByUrgency(all.Where(r => r.RequestType != RequestType.Complaint));
    }

    public async Task<IEnumerable<Request>> GetAssignedRequestsAsync(int staffId)
    {
        var all = await _requestRepository.GetByAssignedToAsync(staffId);
        return SortByUrgency(all.Where(r => r.RequestType != RequestType.Complaint));
    }

    public async Task<IEnumerable<Request>> GetComplaintsAsync()
        => SortByUrgency(await _requestRepository.GetComplaintsAsync());

    public async Task<Request> CreateRequestAsync(Request request, List<IFormFile>? attachments)
    {
        request.RequestNumber = await _requestRepository.GenerateRequestNumberAsync();
        request.Status = RequestStatus.Pending;
        request.CreatedAt = DateTime.Now;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _requestRepository.AddAsync(request);

            if (attachments != null && attachments.Count > 0)
            {
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "requests", request.RequestId.ToString());
                Directory.CreateDirectory(uploadPath);

                foreach (var file in attachments)
                {
                    if (file.Length == 0) continue;

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    await _attachmentRepository.AddAsync(new RequestAttachment
                    {
                        RequestId = request.RequestId,
                        FileName = file.FileName,
                        FilePath = $"/uploads/requests/{request.RequestId}/{fileName}",
                        FileSize = file.Length,
                        ContentType = file.ContentType,
                        UploadedAt = DateTime.Now
                    });
                }
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return request;
    }

    public async Task AssignRequestAsync(int requestId, int staffId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId)
            ?? throw new InvalidOperationException("Không tìm thấy yêu cầu.");

        request.AssignedTo = staffId;
        request.Status = RequestStatus.InProgress;
        request.UpdatedAt = DateTime.Now;

        await _requestRepository.UpdateAsync(request);
    }

    public async Task UpdateStatusAsync(int requestId, RequestStatus status)
    {
        var request = await _requestRepository.GetByIdAsync(requestId)
            ?? throw new InvalidOperationException("Không tìm thấy yêu cầu.");

        request.Status = status;
        request.UpdatedAt = DateTime.Now;

        if (status == RequestStatus.Completed)
            request.ResolvedAt = DateTime.Now;

        await _requestRepository.UpdateAsync(request);
    }

    public async Task AddCommentAsync(int requestId, int authorId, string content)
    {
        var request = await _requestRepository.GetByIdAsync(requestId)
            ?? throw new InvalidOperationException("Không tìm thấy yêu cầu.");

        if (request.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected)
            throw new InvalidOperationException("Không thể thêm bình luận vào yêu cầu đã đóng.");

        var comment = new RequestComment
        {
            RequestId = requestId,
            AuthorId = authorId,
            Content = content.Trim(),
            CreatedAt = DateTime.Now
        };

        _context.RequestComments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePriorityAsync(int requestId, RequestPriority priority)
    {
        var request = await _requestRepository.GetByIdAsync(requestId)
            ?? throw new InvalidOperationException("Không tìm thấy yêu cầu.");

        request.Priority = priority;
        request.UpdatedAt = DateTime.Now;

        await _requestRepository.UpdateAsync(request);
    }

    public async Task<IEnumerable<Request>> GetAllRequestsAsync(
        RequestStatus? status, RequestType? type, RequestPriority? priority, string? search)
    {
        var allRaw = await _requestRepository.GetAllWithDetailsAsync();
        var all = allRaw.Where(r => r.RequestType != RequestType.Complaint);

        if (status.HasValue)
            all = all.Where(r => r.Status == status.Value);

        if (type.HasValue)
            all = all.Where(r => r.RequestType == type.Value);

        if (priority.HasValue)
            all = all.Where(r => r.Priority == priority.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            all = all.Where(r =>
                r.RequestNumber.ToLower().Contains(keyword) ||
                r.Title.ToLower().Contains(keyword) ||
                (r.Resident?.FullName?.ToLower().Contains(keyword) ?? false) ||
                (r.Apartment?.ApartmentNumber?.ToLower().Contains(keyword) ?? false));
        }

        return SortByUrgency(all);
    }

    public async Task EscalateAsync(int requestId, int escalatedToManagerId, string reason)
    {
        var request = await _requestRepository.GetByIdAsync(requestId)
            ?? throw new InvalidOperationException("Không tìm thấy yêu cầu.");

        if (request.EscalatedAt.HasValue)
            throw new InvalidOperationException("Yêu cầu này đã được chuyển cấp trước đó.");

        if (request.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected)
            throw new InvalidOperationException("Không thể chuyển cấp yêu cầu đã đóng.");

        var managerExists = await _context.Users
            .AnyAsync(u => u.UserId == escalatedToManagerId
                        && u.Role == UserRole.BQL_Manager
                        && u.IsActive
                        && !u.IsDeleted);

        if (!managerExists)
            throw new InvalidOperationException("Quản lý được chọn không hợp lệ hoặc không còn hoạt động.");

        request.EscalatedTo = escalatedToManagerId;
        request.EscalatedAt = DateTime.Now;
        request.EscalationReason = reason.Trim();
        request.UpdatedAt = DateTime.Now;

        await _requestRepository.UpdateAsync(request);
    }

    public async Task ForwardComplaintAsync(int requestId, int managerId, string reason)
    {
        var request = await _requestRepository.GetByIdAsync(requestId)
            ?? throw new InvalidOperationException("Không tìm thấy yêu cầu.");

        if (request.RequestType != RequestType.Complaint)
            throw new InvalidOperationException("Yêu cầu này không phải khiếu nại.");

        if (request.EscalatedAt.HasValue)
            throw new InvalidOperationException("Khiếu nại này đã được chuyển cho BQL trước đó.");

        if (request.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected)
            throw new InvalidOperationException("Không thể chuyển xử lý khiếu nại đã đóng.");

        var managerExists = await _context.Users
            .AnyAsync(u => u.UserId == managerId
                        && u.Role == UserRole.BQL_Manager
                        && u.IsActive
                        && !u.IsDeleted);

        if (!managerExists)
            throw new InvalidOperationException("Quản lý BQL được chọn không hợp lệ hoặc không còn hoạt động.");

        request.EscalatedTo = managerId;
        request.EscalatedAt = DateTime.Now;
        request.EscalationReason = reason.Trim();
        request.Status = RequestStatus.InProgress;
        request.UpdatedAt = DateTime.Now;

        await _requestRepository.UpdateAsync(request);
    }

    public async Task<IEnumerable<Request>> GetForwardedComplaintsAsync(int managerId)
    {
        var all = await _requestRepository.GetAllWithDetailsAsync();
        return SortByUrgency(all.Where(r =>
            r.RequestType == RequestType.Complaint && r.EscalatedTo == managerId));
    }

    public async Task<IEnumerable<Request>> GetEscalatedRequestsAsync()
    {
        var all = await _requestRepository.GetAllWithDetailsAsync();
        return all.Where(r => r.EscalatedAt.HasValue && r.RequestType != RequestType.Complaint);
    }

    /// <summary>
    /// Sắp xếp yêu cầu theo độ tồn đọng:
    /// 1. Trạng thái active (Pending → InProgress) trước, closed xuống cuối
    /// 2. Priority cao hơn trước (Emergency > High > Normal > Low)
    /// 3. Ngày tạo cũ hơn trước (chờ lâu nhất lên đầu tiên)
    /// </summary>
    private static IEnumerable<Request> SortByUrgency(IEnumerable<Request> requests)
    {
        return requests
            .OrderBy(r => r.Status switch
            {
                RequestStatus.Pending    => 0,
                RequestStatus.InProgress => 1,
                RequestStatus.Completed  => 2,
                RequestStatus.Cancelled  => 3,
                RequestStatus.Rejected   => 3,
                _                        => 4
            })
            .ThenByDescending(r => (int)r.Priority)
            .ThenBy(r => r.CreatedAt);
    }
}