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
        => await _requestRepository.GetByResidentIdAsync(residentId);

    public async Task<Request?> GetRequestDetailAsync(int requestId)
        => await _requestRepository.GetWithDetailsAsync(requestId);

    public async Task<IEnumerable<Request>> GetAllRequestsAsync()
        => await _requestRepository.GetAllWithDetailsAsync();

    public async Task<IEnumerable<Request>> GetAssignedRequestsAsync(int staffId)
        => await _requestRepository.GetByAssignedToAsync(staffId);

    public async Task<IEnumerable<Request>> GetComplaintsAsync()
        => await _requestRepository.GetComplaintsAsync();

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
}