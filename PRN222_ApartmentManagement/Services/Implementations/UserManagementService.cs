using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;
using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IAuthService _authService;
    private readonly ApartmentDbContext _context;
    private readonly IResidentApartmentRepository _residentApartmentRepository;
    private readonly IEmailService _emailService;

    private const string DefaultPassword = "123456789";

    public UserManagementService(
        IUserRepository userRepository,
        IApartmentRepository apartmentRepository,
        IAuthService authService,
        ApartmentDbContext context,
        IResidentApartmentRepository residentApartmentRepository,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _apartmentRepository = apartmentRepository;
        _authService = authService;
        _context = context;
        _residentApartmentRepository = residentApartmentRepository;
        _emailService = emailService;
    }

    public async Task<List<Apartment>> GetAssignableApartmentsAsync()
    {
        var apartments = await _apartmentRepository.GetAllAsync();
        return apartments
            .Where(a => a.Status != ApartmentStatus.Reserved)
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

    private async Task<string> GenerateUniqueUsernameAsync(string fullName)
    {
        string username;
        int attempts = 0;
        do
        {
            username = UsernameGenerator.Generate(fullName);
            attempts++;
            if (attempts > 100)
                throw new InvalidOperationException("Không thể tạo username unique sau 100 lần thử.");
        }
        while (await _userRepository.UsernameExistsAsync(username));
        return username;
    }

    private async Task SendFamilyMemberCredentialsEmail(User user, string plainPassword)
    {
        var subject = "Thong tin tai khoan thanh vien gia dinh - He thong quan ly chung cu";
        var body = $@"
<p>Xin chao <strong>{user.FullName}</strong>,</p>
<p>Tai khoan thanh vien gia dinh cua ban da duoc tao boi chu ho.</p>
<p><strong>Thong tin dang nhap:</strong></p>
<ul>
    <li><strong>Tai khoan:</strong> {user.Username}</li>
    <li><strong>Mat khau mac dinh:</strong> {DefaultPassword}</li>
</ul>
<p><strong>Luu y quan trong:</strong></p>
<ul>
    <li>Vui long dang nhap va xac minh so dien thoai bang OTP de kich hoat tai khoan.</li>
    <li>Sau khi kich hoat, ban co the doi mat khau trong trang cai dat tai khoan.</li>
</ul>
<p>Neu co thac mac, vui long lien he chu ho hoac Ban Quan Ly.</p>
<p>Tran trọng,<br/>He thong Quan Ly Chung Cu</p>";
        try
        {
            await _emailService.SendEmailAsync(user.Email ?? "", subject, body);
        }
        catch
        {
        }
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

        if (!user.IsActive)
        {
            await _authService.RevokeAllRefreshTokensAsync(user.UserId, reason: "Account deactivated by admin.");
        }

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
        await _authService.RevokeAllRefreshTokensAsync(user.UserId, reason: "Account soft deleted.");
        return (true, null);
    }

    public async Task<List<User>> GetFamilyMembersAsync(int ownerUserId)
    {
        var ownerResidentApt = await _residentApartmentRepository.GetPrimaryAsync(ownerUserId);
        if (ownerResidentApt == null)
            return new List<User>();

        return await _context.ResidentApartments
            .Where(ra => ra.ApartmentId == ownerResidentApt.ApartmentId
                      && ra.ResidencyType == ResidencyType.FamilyMember
                      && ra.IsActive)
            .Include(ra => ra.User)
            .Select(ra => ra.User!)
            .Where(u => u != null && !u.IsDeleted)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<MyApartmentViewModel>> GetMyApartmentsAsync(int userId)
    {
        var currentUser = await _userRepository.GetActiveByIdAsync(userId);
        var allRecords = await _residentApartmentRepository.GetByUserIdAsync(userId);

        if (currentUser?.ResidentType == ResidentType.FamilyMember)
        {
            allRecords = allRecords.Where(r => r.IsActive).ToList();
        }

        var result = new List<MyApartmentViewModel>();
        foreach (var ra in allRecords)
        {
            var contract = ra.Contract;
            var familyMemberCount = await _context.ResidentApartments
                .CountAsync(r => r.ApartmentId == ra.ApartmentId
                              && r.ResidencyType == ResidencyType.FamilyMember
                              && r.IsActive);

            result.Add(new MyApartmentViewModel
            {
                ApartmentId = ra.ApartmentId,
                ApartmentNumber = ra.Apartment?.ApartmentNumber ?? "",
                Floor = ra.Apartment?.Floor ?? 0,
                BuildingBlock = ra.Apartment?.BuildingBlock,
                Area = ra.Apartment?.Area,
                ApartmentType = ra.Apartment?.ApartmentType,
                Status = ra.Apartment?.Status ?? ApartmentStatus.Available,
                ContractId = ra.ContractId,
                ContractNumber = contract?.ContractNumber,
                ContractType = contract?.ContractType,
                ResidencyType = ra.ResidencyType,
                ContractStartDate = contract?.StartDate,
                ContractEndDate = contract?.EndDate,
                FamilyMemberCount = familyMemberCount,
                ResidentApartmentId = ra.ResidentApartmentId
            });
        }

        return result;
    }

    public async Task<ApartmentDetailViewModel?> GetApartmentWithDetailsAsync(int apartmentId, int userId)
    {
        var ra = await _context.ResidentApartments
            .Include(r => r.Apartment)
            .Include(r => r.Contract)
            .FirstOrDefaultAsync(r => r.ApartmentId == apartmentId && r.UserId == userId);

        if (ra == null) return null;

        var contract = ra.Contract;
        return new ApartmentDetailViewModel
        {
            ApartmentId = ra.ApartmentId,
            ApartmentNumber = ra.Apartment?.ApartmentNumber ?? "",
            Floor = ra.Apartment?.Floor ?? 0,
            BuildingBlock = ra.Apartment?.BuildingBlock,
            Area = ra.Apartment?.Area,
            ApartmentType = ra.Apartment?.ApartmentType,
            Status = ra.Apartment?.Status ?? ApartmentStatus.Available,
            ContractId = ra.ContractId,
            ContractNumber = contract?.ContractNumber,
            ContractType = contract?.ContractType,
            ResidencyType = ra.ResidencyType,
            ContractStartDate = contract?.StartDate,
            ContractEndDate = contract?.EndDate,
            MonthlyRent = contract?.MonthlyRent,
            DepositAmount = contract?.DepositAmount,
            PurchasePrice = contract?.PurchasePrice
        };
    }

    public async Task<List<User>> GetFamilyMembersForApartmentAsync(int apartmentId)
    {
        return await _context.ResidentApartments
            .Where(ra => ra.ApartmentId == apartmentId
                      && ra.ResidencyType == ResidencyType.FamilyMember
                      && ra.IsActive)
            .Include(ra => ra.User)
            .Select(ra => ra.User!)
            .Where(u => u != null && !u.IsDeleted)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ApartmentResidentDto>> GetAllResidentsForApartmentAsync(int apartmentId)
    {
        return await _context.ResidentApartments
            .Where(ra => ra.ApartmentId == apartmentId && ra.IsActive)
            .Include(ra => ra.User)
            .Where(ra => ra.User != null && !ra.User.IsDeleted)
            .OrderByDescending(ra => ra.ResidencyType == ResidencyType.FamilyMember)
            .Select(ra => new ApartmentResidentDto
            {
                UserId = ra.User!.UserId,
                Username = ra.User.Username,
                FullName = ra.User.FullName,
                IdentityCardNumber = ra.User.IdentityCardNumber,
                Email = ra.User.Email,
                PhoneNumber = ra.User.PhoneNumber,
                DateOfBirth = ra.User.DateOfBirth,
                IsActive = ra.User.IsActive,
                CreatedAt = ra.User.CreatedAt,
                ResidencyType = ra.ResidencyType
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string? ErrorField, string? ErrorMessage)> CreateFamilyMemberAsync(
        FamilyMemberCreateDto dto,
        int creatorResidentUserId,
        int apartmentId)
    {
        if (await _userRepository.IdentityCardExistsAsync(dto.IdentityCardNumber))
        {
            return (false, "Input.IdentityCardNumber", "Số CCCD đã được sử dụng.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) && await _userRepository.EmailExistsAsync(dto.Email))
        {
            return (false, "Input.Email", "Email đã được sử dụng.");
        }

        if (await _userRepository.PhoneExistsAsync(dto.PhoneNumber))
        {
            return (false, "Input.PhoneNumber", "Số điện thoại đã được sử dụng.");
        }

        var contract = await _context.Contracts
            .Where(c => c.ApartmentId == apartmentId && c.Status == ContractStatus.Active)
            .FirstOrDefaultAsync();
        if (contract == null)
        {
            return (false, null, "Không tìm thấy hợp đồng đang hoạt động cho căn hộ này.");
        }

        var username = await GenerateUniqueUsernameAsync(dto.FullName);

        var familyMember = new User
        {
            Username = username,
            PasswordHash = BC.HashPassword(DefaultPassword),
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth,
            IdentityCardNumber = dto.IdentityCardNumber,
            ResidentType = ResidentType.FamilyMember,
            Role = UserRole.Resident,
            IsActive = false,
            CreatedAt = DateTime.Now
        };

        await _userRepository.AddAsync(familyMember);
        await _context.SaveChangesAsync();

        var residentApt = new ResidentApartment
        {
            UserId = familyMember.UserId,
            ApartmentId = apartmentId,
            ContractId = contract.ContractId,
            ResidencyType = ResidencyType.FamilyMember,
            IsActive = true,
            MoveInDate = DateTime.Now,
            CreatedAt = DateTime.Now
        };

        await _context.ResidentApartments.AddAsync(residentApt);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            await SendFamilyMemberCredentialsEmail(familyMember, DefaultPassword);
        }

        return (true, null, null);
    }
}
