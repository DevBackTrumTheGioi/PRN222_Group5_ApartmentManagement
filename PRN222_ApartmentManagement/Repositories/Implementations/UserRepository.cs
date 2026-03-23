using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApartmentDbContext context, IActivityLogService activityLog)
        : base(context, activityLog)
    {
    }

    public async Task<User?> GetActiveByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username && !u.IsDeleted);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        return await _dbSet.AnyAsync(u =>
            u.Email == email &&
            !u.IsDeleted &&
            (!excludeUserId.HasValue || u.UserId != excludeUserId.Value));
    }

    public async Task<bool> PhoneExistsAsync(string phone, int? excludeUserId = null)
    {
        return await _dbSet.AnyAsync(u =>
            u.PhoneNumber == phone &&
            !u.IsDeleted &&
            (!excludeUserId.HasValue || u.UserId != excludeUserId.Value));
    }

    public async Task<bool> IdentityCardExistsAsync(string cccd, int? excludeUserId = null)
    {
        return await _dbSet.AnyAsync(u =>
            u.IdentityCardNumber == cccd &&
            !u.IsDeleted &&
            (!excludeUserId.HasValue || u.UserId != excludeUserId.Value));
    }

    public async Task<User?> FindByPhoneAsync(string phone)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phone && !u.IsDeleted);
    }

    public async Task<User?> FindByIdentityCardAsync(string cccd)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.IdentityCardNumber == cccd && !u.IsDeleted);
    }

    public async Task<List<User>> GetPagedUsersAsync(string? searchTerm, UserRole? roleFilter, int pageIndex, int pageSize)
    {
        var query = BuildUserQuery(searchTerm, roleFilter);

        return await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountUsersAsync(string? searchTerm, UserRole? roleFilter)
    {
        return await BuildUserQuery(searchTerm, roleFilter).CountAsync();
    }

    private IQueryable<User> BuildUserQuery(string? searchTerm, UserRole? roleFilter)
    {
        var query = _dbSet.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.FullName.Contains(searchTerm) ||
                u.Username.Contains(searchTerm) ||
                (u.Email != null && u.Email.Contains(searchTerm)));
        }

        if (roleFilter.HasValue)
        {
            query = query.Where(u => u.Role == roleFilter.Value);
        }

        return query;
    }

    public async Task<PagedResult<User>> GetPagedResidentsAsync(
        string? searchTerm,
        bool? isActive,
        int pageIndex,
        int pageSize)
    {
        var baseQuery = _dbSet
            .Where(u => !u.IsDeleted && u.Role == UserRole.Resident);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            baseQuery = baseQuery.Where(u =>
                u.FullName.Contains(term) ||
                u.Username.Contains(term) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(term)) ||
                (u.IdentityCardNumber != null && u.IdentityCardNumber.Contains(term)));
        }

        if (isActive.HasValue)
            baseQuery = baseQuery.Where(u => u.IsActive == isActive.Value);

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<User?> GetResidentByIdWithDetailsAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.Apartment)
            .Include(u => u.Vehicles.Where(v => !v.IsDeleted))
            .Include(u => u.ResidentCards)
            .Include(u => u.Requests.OrderByDescending(r => r.CreatedAt).Take(5))
            .Include(u => u.ContractMembers.Where(cm => cm.IsActive))
                .ThenInclude(cm => cm.Contract)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted && u.Role == UserRole.Resident);
    }
}
