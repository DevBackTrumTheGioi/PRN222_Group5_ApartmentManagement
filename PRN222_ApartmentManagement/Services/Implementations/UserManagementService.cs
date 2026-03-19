using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IAuthService _authService;

    public UserManagementService(
        IUserRepository userRepository,
        IApartmentRepository apartmentRepository,
        IAuthService authService)
    {
        _userRepository = userRepository;
        _apartmentRepository = apartmentRepository;
        _authService = authService;
    }

    public async Task<List<Apartment>> GetAssignableApartmentsAsync()
    {
        var apartments = await _apartmentRepository.GetAllAsync();
        return apartments
            .Where(a => a.Status != ApartmentStatus.Maintenance)
            .OrderBy(a => a.ApartmentNumber)
            .ToList();
    }

    public async Task<(List<User> Users, int TotalItems, int TotalPages)> GetPagedUsersAsync(
        string? searchTerm,
        string? roleFilter,
        int pageIndex,
        int pageSize)
    {
        UserRole? parsedRole = null;
        if (!string.IsNullOrWhiteSpace(roleFilter) && Enum.TryParse<UserRole>(roleFilter, out var role))
        {
            parsedRole = role;
        }

        var totalItems = await _userRepository.CountUsersAsync(searchTerm, parsedRole);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (pageIndex < 1)
        {
            pageIndex = 1;
        }

        if (pageIndex > totalPages && totalPages > 0)
        {
            pageIndex = totalPages;
        }

        var users = await _userRepository.GetPagedUsersAsync(searchTerm, parsedRole, pageIndex, pageSize);
        return (users, totalItems, totalPages);
    }

    public Task<User?> GetUserForEditAsync(int userId)
    {
        return _userRepository.GetActiveByIdAsync(userId);
    }

    public async Task<(bool Success, string? ErrorField, string? ErrorMessage)> CreateUserAsync(UserUpsertRequest request)
    {
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            return (false, "Input.Username", "Tên đăng nhập đã tồn tại.");
        }

        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return (false, "Input.Email", "Email đã được sử dụng.");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = _authService.HashPassword(request.Password ?? string.Empty),
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            IsActive = request.IsActive,
            CreatedAt = DateTime.Now,
            DateOfBirth = request.DateOfBirth,
            IdentityCardNumber = request.IdentityCardNumber,
            ResidentType = request.ResidentType,
            ResidencyStatus = request.ResidencyStatus,
            ApartmentId = request.ApartmentId,
            MoveInDate = request.MoveInDate,
            MoveOutDate = request.MoveOutDate,
            Note = request.Note
        };

        await _userRepository.AddAsync(user);
        return (true, null, null);
    }

    public async Task<(bool Success, string? ErrorField, string? ErrorMessage)> UpdateUserAsync(UserUpsertRequest request)
    {
        if (!request.UserId.HasValue)
        {
            return (false, null, "Thiếu thông tin người dùng.");
        }

        var user = await _userRepository.GetActiveByIdAsync(request.UserId.Value);
        if (user == null)
        {
            return (false, null, "Không tìm thấy người dùng.");
        }

        if (await _userRepository.EmailExistsAsync(request.Email, request.UserId.Value))
        {
            return (false, "Input.Email", "Email đã được sử dụng bởi người dùng khác.");
        }

        user.FullName = request.FullName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.Now;
        user.DateOfBirth = request.DateOfBirth;
        user.IdentityCardNumber = request.IdentityCardNumber;
        user.ResidentType = request.ResidentType;
        user.ResidencyStatus = request.ResidencyStatus;
        user.ApartmentId = request.ApartmentId;
        user.MoveInDate = request.MoveInDate;
        user.MoveOutDate = request.MoveOutDate;
        user.Note = request.Note;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = _authService.HashPassword(request.Password);
        }

        await _userRepository.UpdateAsync(user);
        return (true, null, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> ToggleStatusAsync(int userId)
    {
        var user = await _userRepository.GetActiveByIdAsync(userId);
        if (user == null)
        {
            return (false, "Không tìm thấy người dùng.");
        }

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.Now;
        await _userRepository.UpdateAsync(user);
        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> SoftDeleteAsync(int userId)
    {
        var user = await _userRepository.GetActiveByIdAsync(userId);
        if (user == null)
        {
            return (false, "Không tìm thấy người dùng.");
        }

        user.IsDeleted = true;
        user.IsActive = false;
        user.UpdatedAt = DateTime.Now;
        await _userRepository.UpdateAsync(user);
        return (true, null);
    }
}
