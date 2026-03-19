using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetActiveByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    Task<List<User>> GetPagedUsersAsync(string? searchTerm, UserRole? roleFilter, int pageIndex, int pageSize);
    Task<int> CountUsersAsync(string? searchTerm, UserRole? roleFilter);
}

