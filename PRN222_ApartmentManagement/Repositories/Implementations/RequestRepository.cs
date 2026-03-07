using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class RequestRepository : GenericRepository<Request>, IRequestRepository
{
    public RequestRepository(ApartmentDbContext context, IActivityLogService activityLog)
        : base(context, activityLog)
    {
    }

    public async Task<IEnumerable<Request>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(r => r.Resident)
            .Include(r => r.Apartment)
            .Include(r => r.AssignedUser)
            .Include(r => r.RequestAttachments)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Request?> GetWithDetailsAsync(int requestId)
    {
        return await _dbSet
            .Include(r => r.Resident)
            .Include(r => r.Apartment)
            .Include(r => r.AssignedUser)
            .Include(r => r.EscalatedToUser)
            .Include(r => r.RequestAttachments)
            .FirstOrDefaultAsync(r => r.RequestId == requestId);
    }

    public async Task<IEnumerable<Request>> GetByResidentIdAsync(int residentId)
    {
        return await _dbSet
            .Include(r => r.Apartment)
            .Include(r => r.AssignedUser)
            .Include(r => r.RequestAttachments)
            .Where(r => r.ResidentId == residentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Request>> GetByAssignedToAsync(int staffId)
    {
        return await _dbSet
            .Include(r => r.Resident)
            .Include(r => r.Apartment)
            .Include(r => r.RequestAttachments)
            .Where(r => r.AssignedTo == staffId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Request>> GetByStatusAsync(RequestStatus status)
    {
        return await _dbSet
            .Include(r => r.Resident)
            .Include(r => r.Apartment)
            .Include(r => r.AssignedUser)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Request>> GetComplaintsAsync()
    {
        return await _dbSet
            .Include(r => r.Resident)
            .Include(r => r.Apartment)
            .Where(r => r.RequestType == RequestType.Complaint)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<string> GenerateRequestNumberAsync()
    {
        var prefix = $"REQ-{DateTime.Now:yyyyMM}-";

        var lastRequest = await _dbSet
            .Where(r => r.RequestNumber.StartsWith(prefix))
            .OrderByDescending(r => r.RequestNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastRequest != null)
        {
            var lastNumberStr = lastRequest.RequestNumber.Replace(prefix, "");
            if (int.TryParse(lastNumberStr, out int lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D4}";
    }
}