using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class ContractRepository : GenericRepository<Contract>, IContractRepository
{
    public ContractRepository(ApartmentDbContext context, IActivityLogService activityLog) : base(context, activityLog)
    {
    }

    public async Task<PagedResult<Contract>> GetPagedFilteredAsync(
        string? searchTerm,
        ContractStatus? status,
        ContractType? contractType,
        int pageIndex,
        int pageSize)
    {
        // Build filter trên base query KHÔNG include (COUNT cần clean query)
        var baseQuery = _context.Contracts
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            baseQuery = baseQuery.Where(c =>
                c.ContractNumber.Contains(term) ||
                c.Apartment.ApartmentNumber.Contains(term) ||
                c.ContractMembers.Any(cm =>
                    cm.Resident != null && cm.Resident.FullName.Contains(term)));
        }

        if (status.HasValue)
            baseQuery = baseQuery.Where(c => c.Status == status.Value);

        if (contractType.HasValue)
            baseQuery = baseQuery.Where(c => c.ContractType == contractType.Value);

        // COUNT trên clean query — không bị ảnh hưởng bởi Include
        var totalCount = await baseQuery.CountAsync();

        // Include chỉ needed fields khi lấy page data
        var items = await baseQuery
            .Include(c => c.Apartment)
            .Include(c => c.ContractMembers.Where(cm => cm.MemberRole == MemberRole.ContractOwner))
                .ThenInclude(cm => cm.Resident)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Contract>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }
}

