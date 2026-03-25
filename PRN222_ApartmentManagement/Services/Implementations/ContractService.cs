using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;
using BC = BCrypt.Net.BCrypt;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class ContractService : IContractService
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractMemberRepository _contractMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IResidentApartmentRepository _residentApartmentRepository;
    private readonly IActivityLogService _activityLogService;
    private readonly ApartmentDbContext _dbContext;

    private const string DefaultPassword = "123456789";

    public ContractService(
        IContractRepository contractRepository,
        IContractMemberRepository contractMemberRepository,
        IUserRepository userRepository,
        IApartmentRepository apartmentRepository,
        IResidentApartmentRepository residentApartmentRepository,
        IActivityLogService activityLogService,
        ApartmentDbContext dbContext)
    {
        _contractRepository = contractRepository;
        _contractMemberRepository = contractMemberRepository;
        _userRepository = userRepository;
        _apartmentRepository = apartmentRepository;
        _residentApartmentRepository = residentApartmentRepository;
        _activityLogService = activityLogService;
        _dbContext = dbContext;
    }

    public async Task<List<Contract>> GetAllAsync()
    {
        return await _dbContext.Contracts
            .Where(c => !c.IsDeleted)
            .Include(c => c.Apartment)
            .Include(c => c.Creator)
            .Include(c => c.ContractMembers)
                .ThenInclude(cm => cm.Resident)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<PagedResult<Contract>> GetPagedFilteredAsync(
        string? searchTerm,
        ContractStatus? status,
        ContractType? contractType,
        int pageIndex,
        int pageSize)
    {
        return await _contractRepository.GetPagedFilteredAsync(searchTerm, status, contractType, pageIndex, pageSize);
    }

    public async Task<Contract?> GetByIdAsync(int id)
    {
        return await _dbContext.Contracts
            .Where(c => !c.IsDeleted && c.ContractId == id)
            .Include(c => c.Apartment)
            .Include(c => c.Creator)
            .FirstOrDefaultAsync();
    }

    public async Task<Contract?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbContext.Contracts
            .Where(c => !c.IsDeleted && c.ContractId == id)
            .Include(c => c.Apartment)
            .Include(c => c.Creator)
            .Include(c => c.ContractMembers)
                .ThenInclude(cm => cm.Resident)
            .FirstOrDefaultAsync();
    }

    public async Task<Contract> CreateContractAsync(CreateContractDto dto, int creatorId)
    {
        var contractNumber = await GenerateContractNumberAsync(dto.ContractType!.Value);

        var contract = new Contract
        {
            ContractNumber = contractNumber,
            ApartmentId = dto.ApartmentId,
            ContractType = dto.ContractType,
            StartDate = dto.StartDate,
            EndDate = dto.ContractType == ContractType.Rental ? dto.EndDate : null,
            MonthlyRent = dto.ContractType == ContractType.Rental ? dto.MonthlyRent : null,
            DepositAmount = dto.ContractType == ContractType.Rental ? dto.DepositAmount : null,
            PurchasePrice = dto.ContractType == ContractType.Purchase ? dto.PurchasePrice : null,
            Terms = dto.Terms,
            Status = ContractStatus.Draft,
            CreatedBy = creatorId,
            CreatedAt = DateTime.Now,
            OwnerFullName = dto.OwnerFullName,
            OwnerEmail = dto.OwnerEmail,
            OwnerPhone = dto.OwnerPhone,
            OwnerDateOfBirth = dto.OwnerDateOfBirth,
            OwnerIdentityCard = dto.IsExistingOwner
                ? dto.ExistingOwnerCccd
                : dto.OwnerIdentityCard
        };

        await _contractRepository.AddAsync(contract);
        await _dbContext.SaveChangesAsync();

        // Khi tao hop dong (Draft), chuyen trang thai chung cu sang Da dat
        var apartment = await _dbContext.Apartments.FindAsync(dto.ApartmentId);
        if (apartment != null)
        {
            apartment.Status = ApartmentStatus.Reserved;
            apartment.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }

        await _activityLogService.LogCustomAsync(
            "CreateContract",
            nameof(Contract),
            contract.ContractId.ToString(),
            $"Tao hop dong {contract.ContractNumber} (Ban nap). Vui long kich hoat de tao tai khoan chu ho.");

        return contract;
    }

    public async Task<Contract?> ApproveContractAsync(int contractId, int approvedBy)
    {
        var contract = await _dbContext.Contracts
            .Include(c => c.ContractMembers)
            .FirstOrDefaultAsync(c => c.ContractId == contractId && !c.IsDeleted);

        if (contract == null)
            return null;

        if (contract.Status != ContractStatus.Draft)
            throw new InvalidOperationException("Chi hop dong o trang thai Ban nap moi duoc kich hoat.");

        if (string.IsNullOrWhiteSpace(contract.OwnerFullName))
            throw new InvalidOperationException("Thong tin chu ho chua duoc luu. Vui long chinh sua hop dong truoc.");

        await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var cccd = contract.OwnerIdentityCard?.Trim();
            User? existingByCccd = null;
            if (!string.IsNullOrWhiteSpace(cccd))
            {
                existingByCccd = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.IdentityCardNumber == cccd && !u.IsDeleted);
            }

            int ownerUserId;
            string ownerUsername;

            if (existingByCccd != null)
            {
                // Chủ hộ đã có tài khoản (cùng CCCD) — chỉ liên kết hợp đồng, không tạo user trùng
                ownerUserId = existingByCccd.UserId;
                ownerUsername = existingByCccd.Username;
            }
            else
            {
                var username = await GenerateUniqueUsernameAsync(contract.OwnerFullName!);
                var ownerUser = new User
                {
                    Username = username,
                    PasswordHash = BC.HashPassword(DefaultPassword),
                    FullName = contract.OwnerFullName!,
                    Email = contract.OwnerEmail,
                    PhoneNumber = contract.OwnerPhone,
                    DateOfBirth = contract.OwnerDateOfBirth,
                    IdentityCardNumber = contract.OwnerIdentityCard,
                    ResidentType = ResidentType.Owner,
                    Role = UserRole.Resident,
                    IsActive = false,
                    CreatedAt = DateTime.Now
                };
                await _userRepository.AddAsync(ownerUser);
                await _dbContext.SaveChangesAsync();
                ownerUserId = ownerUser.UserId;
                ownerUsername = username;
            }

            // Tao ContractMember
            var contractMember = new ContractMember
            {
                ContractId = contract.ContractId,
                ResidentId = ownerUserId,
                MemberRole = MemberRole.ContractOwner,
                SignatureStatus = SignatureStatus.Signed,
                SignedDate = DateTime.Now,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            await _contractMemberRepository.AddAsync(contractMember);

            // Tao ResidentApartment record — ghi nhan lich su cu tru cua user tai apartment nay
            var residencyType = contract.ContractType switch
            {
                ContractType.Rental => ResidencyType.Tenant,
                ContractType.Purchase => ResidencyType.Owner,
                _ => ResidencyType.Other
            };

            var residentApartment = new ResidentApartment
            {
                UserId = ownerUserId,
                ApartmentId = contract.ApartmentId,
                ContractId = contract.ContractId,
                ResidencyType = residencyType,
                IsActive = true,
                MoveInDate = contract.StartDate,
                CreatedAt = DateTime.Now
            };
            await _residentApartmentRepository.AddAsync(residentApartment);

            // Cap nhat User.ApartmentId = apartment moi nhat (tien ich, con khop voi code cu)
            var ownerUserForUpdate = await _dbContext.Users.FindAsync(ownerUserId);
            if (ownerUserForUpdate != null)
            {
                ownerUserForUpdate.ApartmentId = contract.ApartmentId;
                ownerUserForUpdate.UpdatedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.SaveChangesAsync();

            // Chuyen trang thai hop dong sang Active
            contract.Status = ContractStatus.Active;
            contract.SignedDate = DateTime.Now;
            contract.UpdatedAt = DateTime.Now;
            await _contractRepository.UpdateAsync(contract);

            // Chuyen trang thai chung cu sang Occupied khi hop dong co hieu luc
            var apt = await _dbContext.Apartments.FindAsync(contract.ApartmentId);
            if (apt != null)
            {
                apt.Status = ApartmentStatus.Occupied;
                apt.UpdatedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.Database.CommitTransactionAsync();

            await _activityLogService.LogCustomAsync(
                "ApproveContract",
                nameof(Contract),
                contract.ContractId.ToString(),
                $"Kich hoat hop dong {contract.ContractNumber}. Chu ho (tai khoan): {ownerUsername}. Nguoi duyet: {approvedBy}.");

            return contract;
        }
        catch
        {
            await _dbContext.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Contract?> UpdateContractAsync(int id, UpdateContractDto dto)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null || contract.IsDeleted)
            return null;

        if (dto.ContractType.HasValue) contract.ContractType = dto.ContractType;
        if (dto.StartDate.HasValue) contract.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) contract.EndDate = dto.EndDate;
        if (dto.MonthlyRent.HasValue) contract.MonthlyRent = dto.MonthlyRent;
        if (dto.DepositAmount.HasValue) contract.DepositAmount = dto.DepositAmount;
        if (dto.Terms != null) contract.Terms = dto.Terms;
        if (dto.Status.HasValue) contract.Status = dto.Status.Value;
        if (dto.PurchasePrice.HasValue) contract.PurchasePrice = dto.PurchasePrice;

        // Cập nhật chủ hộ (luu vao Contract de khi kich hoat con lay ra tao tai khoan)
        if (dto.Owner != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.Owner.FullName)) contract.OwnerFullName = dto.Owner.FullName;
            if (dto.Owner.PhoneNumber != null) contract.OwnerPhone = dto.Owner.PhoneNumber;
            if (dto.Owner.Email != null) contract.OwnerEmail = dto.Owner.Email;
            if (dto.Owner.DateOfBirth.HasValue) contract.OwnerDateOfBirth = dto.Owner.DateOfBirth;
            if (dto.IsExistingOwner && !string.IsNullOrWhiteSpace(dto.ExistingOwnerCccd))
                contract.OwnerIdentityCard = dto.ExistingOwnerCccd;
            else if (dto.Owner.IdentityCardNumber != null)
                contract.OwnerIdentityCard = dto.Owner.IdentityCardNumber;
        }

        contract.UpdatedAt = DateTime.Now;
        await _contractRepository.UpdateAsync(contract);

        await _activityLogService.LogCustomAsync(
            "UpdateContract",
            nameof(Contract),
            contract.ContractId.ToString(),
            $"Cap nhat hop dong {contract.ContractNumber}");

        return contract;
    }

    public async Task<(bool Success, string Message)> TerminateContractAsync(int id, string reason, int terminatedBy)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null || contract.IsDeleted)
            return (false, "Khong tim thay hop dong.");

        contract.Status = ContractStatus.Terminated;
        contract.TerminationReason = reason;
        contract.TerminatedAt = DateTime.Now;
        contract.UpdatedAt = DateTime.Now;

        await _contractRepository.UpdateAsync(contract);

        var members = await _dbContext.ContractMembers
            .Where(cm => cm.ContractId == id && cm.IsActive)
            .Include(cm => cm.Resident)
            .ToListAsync();

        foreach (var member in members)
        {
            member.IsActive = false;
            await _contractMemberRepository.UpdateAsync(member);

            if (member.Resident == null) continue;

            // Dong ResidentApartment record cua member cho hop dong nay
            var residentApts = await _dbContext.ResidentApartments
                .Where(ra => ra.UserId == member.ResidentId
                          && ra.ContractId == id
                          && ra.IsActive)
                .ToListAsync();

            foreach (var ra in residentApts)
            {
                ra.IsActive = false;
                ra.MoveOutDate = DateTime.Now;
                ra.UpdatedAt = DateTime.Now;
            }

            var hasOtherActiveContract = await _dbContext.ContractMembers
                .AnyAsync(cm =>
                    cm.ResidentId == member.ResidentId &&
                    cm.IsActive &&
                    cm.ContractId != id);

            if (!hasOtherActiveContract)
            {
                member.Resident.IsActive = false;
                member.Resident.UpdatedAt = DateTime.Now;
                await _userRepository.UpdateAsync(member.Resident);

                await _activityLogService.LogCustomAsync(
                    "UserDeactivatedOnContractTermination",
                    nameof(User),
                    member.Resident.UserId.ToString(),
                    $"Tai khoan {member.Resident.Username} bi khoa vi khong con hop dong active nao sau khi ket thuc hop dong {contract.ContractNumber}.");
            }
        }

        await _activityLogService.LogCustomAsync(
            "TerminateContract",
            nameof(Contract),
            contract.ContractId.ToString(),
            $"Ket thuc hop dong {contract.ContractNumber}. Ly do: {reason}. Nguoi thuc hien: {terminatedBy}");

        // Sau khi ket thuc, kiem tra con ResidentApartment active nao cho apartment nay khong
        var stillHasActive = await _dbContext.ResidentApartments
            .AnyAsync(ra => ra.ApartmentId == contract.ApartmentId && ra.IsActive);
        if (!stillHasActive)
        {
            var apt = await _dbContext.Apartments.FindAsync(contract.ApartmentId);
            if (apt != null)
            {
                apt.Status = ApartmentStatus.Available;
                apt.UpdatedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
        }

        return (true, $"Da ket thuc hop dong {contract.ContractNumber} thanh cong.");
    }

    public async Task<(bool Success, string Message)> DeleteContractAsync(int id, int deletedBy)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null || contract.IsDeleted)
            return (false, "Khong tim thay hop dong.");

        if (contract.Status != ContractStatus.Draft)
            return (false, "Chỉ có thể xóa hợp đồng ở trạng thái bản nháp");

        // Tra trang thai chung cu ve Available khi xoa hop dong Draft
        var apt = await _dbContext.Apartments.FindAsync(contract.ApartmentId);
        if (apt != null)
        {
            apt.Status = ApartmentStatus.Available;
            apt.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }

        contract.IsDeleted = true;
        contract.UpdatedAt = DateTime.Now;

        await _contractRepository.UpdateAsync(contract);

        await _activityLogService.LogCustomAsync(
            "DeleteContract",
            nameof(Contract),
            contract.ContractId.ToString(),
            $"Xoa hop dong {contract.ContractNumber} (ban nap). Nguoi thuc hien: {deletedBy}");

        return (true, $"Da xoa hop dong {contract.ContractNumber} thanh cong.");
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
                throw new InvalidOperationException("Khong the tao username unique sau 100 lan thu.");
        }
        while (await _userRepository.UsernameExistsAsync(username));

        return username;
    }

    public async Task<string> GenerateContractNumberAsync(ContractType type)
    {
        var prefix = type switch
        {
            ContractType.Purchase => "HDM",
            ContractType.Rental => "HDT",
            _ => "HD"
        };
        var datePart = DateTime.Now.ToString("yyyyMMdd");
        var existing = await _contractRepository.FindAsync(c =>
            c.ContractNumber.StartsWith($"{prefix}-{datePart}"));
        var next = (existing.Count() + 1).ToString("D3");
        return $"{prefix}-{datePart}-{next}";
    }

    public async Task<User?> GetContractOwnerAsync(int contractId)
    {
        return await _dbContext.ContractMembers
            .Where(cm => cm.ContractId == contractId && cm.MemberRole == MemberRole.ContractOwner)
            .Select(cm => cm.Resident!)
            .FirstOrDefaultAsync();
    }
}
