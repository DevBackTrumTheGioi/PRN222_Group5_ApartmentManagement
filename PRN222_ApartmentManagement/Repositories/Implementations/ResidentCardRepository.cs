using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class ResidentCardRepository : GenericRepository<ResidentCard>, IResidentCardRepository
{
    public ResidentCardRepository(ApartmentDbContext context, IActivityLogService activityLog)
        : base(context, activityLog) { }

    public async Task<PagedResult<ResidentCard>> GetPagedAsync(
        string? searchTerm,
        CardStatus? status,
        int? apartmentId,
        int pageIndex,
        int pageSize)
    {
        var query = _dbSet
            .Include(rc => rc.Resident)
            .Include(rc => rc.Apartment)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(rc =>
                rc.CardNumber.ToLower().Contains(term) ||
                (rc.Resident.FullName != null && rc.Resident.FullName.ToLower().Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(rc => rc.Status == status.Value);
        }

        if (apartmentId.HasValue)
        {
            query = query.Where(rc => rc.ApartmentId == apartmentId.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(rc => rc.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ResidentCard>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<ResidentCard?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(rc => rc.Resident)
            .Include(rc => rc.Apartment)
            .FirstOrDefaultAsync(rc => rc.ResidentCardId == id);
    }

    public async Task<string> GenerateCardNumberAsync()
    {
        var today = DateTime.Today;
        var prefix = $"RC-{today:yyyyMMdd}";

        var lastCard = await _dbSet
            .Where(rc => rc.CardNumber.StartsWith(prefix))
            .OrderByDescending(rc => rc.CardNumber)
            .FirstOrDefaultAsync();

        int nextSeq = 1;
        if (lastCard != null)
        {
            var lastSeqStr = lastCard.CardNumber.Split('-').LastOrDefault();
            if (int.TryParse(lastSeqStr, out var lastSeq))
            {
                nextSeq = lastSeq + 1;
            }
        }

        return $"{prefix}-{nextSeq:D4}";
    }

    public override async Task<IEnumerable<ResidentCard>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
}
