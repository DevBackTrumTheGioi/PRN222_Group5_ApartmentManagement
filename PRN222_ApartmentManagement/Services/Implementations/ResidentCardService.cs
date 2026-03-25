using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class ResidentCardService : IResidentCardService
{
    private readonly IResidentCardRepository _residentCardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IActivityLogService _activityLogService;
    private readonly ApartmentDbContext _dbContext;

    public ResidentCardService(
        IResidentCardRepository residentCardRepository,
        IUserRepository userRepository,
        IApartmentRepository apartmentRepository,
        IActivityLogService activityLogService,
        ApartmentDbContext dbContext)
    {
        _residentCardRepository = residentCardRepository;
        _userRepository = userRepository;
        _apartmentRepository = apartmentRepository;
        _activityLogService = activityLogService;
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ResidentCard>> GetPagedAsync(
        string? searchTerm,
        CardStatus? status,
        int? apartmentId,
        int pageIndex,
        int pageSize)
    {
        return await _residentCardRepository.GetPagedAsync(searchTerm, status, apartmentId, pageIndex, pageSize);
    }

    public async Task<ResidentCard?> GetByIdAsync(int id)
    {
        return await _residentCardRepository.GetByIdAsync(id);
    }

    public async Task<ResidentCard?> GetByIdWithDetailsAsync(int id)
    {
        return await _residentCardRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task<(bool Success, string Message)> CreateAsync(CreateResidentCardDto dto, int creatorId)
    {
        var resident = await _userRepository.GetByIdAsync(dto.ResidentId);
        if (resident == null || resident.IsDeleted)
            return (false, "Không tìm thấy cư dân.");

        if (!resident.IsActive)
            return (false, "Tài khoản cư dân chưa được kích hoạt.");

        if (dto.ApartmentId.HasValue)
        {
            var apartment = await _apartmentRepository.GetByIdAsync(dto.ApartmentId.Value);
            if (apartment == null)
                return (false, "Không tìm thấy căn hộ.");
        }

        var cardNumber = await _residentCardRepository.GenerateCardNumberAsync();

        var card = new ResidentCard
        {
            CardNumber = cardNumber,
            ResidentId = dto.ResidentId,
            ApartmentId = dto.ApartmentId,
            CardType = dto.CardType ?? CardType.Resident,
            IssuedDate = DateTime.Today,
            ExpiryDate = dto.ExpiryDate,
            Status = CardStatus.Active,
            Notes = dto.Notes,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };

        await _residentCardRepository.AddAsync(card);
        await _dbContext.SaveChangesAsync();

        await _activityLogService.LogCustomAsync(
            "CreateResidentCard",
            nameof(ResidentCard),
            card.ResidentCardId.ToString(),
            $"Tạo thẻ căn hộ {card.CardNumber} cho {resident.FullName}. Người tạo: {creatorId}.");

        return (true, $"Tạo thẻ {card.CardNumber} thành công.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(int id, UpdateResidentCardDto dto)
    {
        var card = await _residentCardRepository.GetByIdAsync(id);
        if (card == null || card.IsDeleted)
            return (false, "Không tìm thấy thẻ.");

        // Neu la the het han, cho phep gia han (dat lai ExpiryDate → Active)
        if (card.Status == CardStatus.Expired && dto.ExpiryDate.HasValue)
        {
            card.Status = CardStatus.Active;
        }

        if (dto.ExpiryDate.HasValue) card.ExpiryDate = dto.ExpiryDate;
        if (dto.Notes != null) card.Notes = dto.Notes;

        card.UpdatedAt = DateTime.Now;
        await _residentCardRepository.UpdateAsync(card);

        await _activityLogService.LogCustomAsync(
            "UpdateResidentCard",
            nameof(ResidentCard),
            card.ResidentCardId.ToString(),
            $"Cập nhật thẻ {card.CardNumber}.");

        return (true, $"Cập nhật thẻ {card.CardNumber} thành công.");
    }

    public async Task<(bool Success, string Message)> UpdateStatusAsync(int id, CardStatus status)
    {
        var card = await _residentCardRepository.GetByIdAsync(id);
        if (card == null || card.IsDeleted)
            return (false, "Không tìm thấy thẻ.");

        card.Status = status;
        card.UpdatedAt = DateTime.Now;
        await _residentCardRepository.UpdateAsync(card);

        var statusLabel = status switch
        {
            CardStatus.Active => "Hoạt động",
            CardStatus.Locked => "Khóa",
            CardStatus.Lost => "Mất",
            CardStatus.Expired => "Hết hạn",
            _ => status.ToString()
        };

        await _activityLogService.LogCustomAsync(
            "UpdateResidentCardStatus",
            nameof(ResidentCard),
            card.ResidentCardId.ToString(),
            $"Đổi trạng thái thẻ {card.CardNumber} thành {statusLabel}.");

        return (true, $"Đổi trạng thái thẻ thành {statusLabel}.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var card = await _residentCardRepository.GetByIdAsync(id);
        if (card == null || card.IsDeleted)
            return (false, "Không tìm thấy thẻ.");

        card.IsDeleted = true;
        card.Status = CardStatus.Lost;
        card.UpdatedAt = DateTime.Now;
        await _residentCardRepository.UpdateAsync(card);

        await _activityLogService.LogCustomAsync(
            "DeleteResidentCard",
            nameof(ResidentCard),
            card.ResidentCardId.ToString(),
            $"Xóa thẻ {card.CardNumber}.");

        return (true, $"Đã xóa thẻ {card.CardNumber}.");
    }

    public async Task<List<Apartment>> GetAvailableApartmentsAsync()
    {
        return await _dbContext.Apartments
            .Where(a => a.Status == ApartmentStatus.Occupied || a.Status == ApartmentStatus.Available)
            .OrderBy(a => a.BuildingBlock)
            .ThenBy(a => a.ApartmentNumber)
            .ToListAsync();
    }

    public async Task<List<User>> GetAvailableResidentsAsync()
    {
        return await _dbContext.Users
            .Where(u => !u.IsDeleted && u.IsActive && u.Role == UserRole.Resident)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<string> GenerateCardNumberAsync()
    {
        return await _residentCardRepository.GenerateCardNumberAsync();
    }

    public async Task<UserSearchResult?> SearchUserAsync(string username)
    {
        var user = await _userRepository.SearchByUsernameAsync(username);
        if (user == null) return null;

        var hasApartments = await _dbContext.ResidentApartments
            .AnyAsync(ra => ra.UserId == user.UserId && ra.IsActive);

        return new UserSearchResult
        {
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Phone = user.PhoneNumber,
            Role = user.Role ?? UserRole.Resident,
            IsStaff = user.Role == UserRole.BQL_Staff,
            HasApartments = hasApartments,
            IsActive = user.IsActive,
            IsDeleted = user.IsDeleted
        };
    }

    public async Task<List<ApartmentWithType>> GetApartmentsByUserAsync(int userId)
    {
        var ras = await _dbContext.ResidentApartments
            .Where(ra => ra.UserId == userId && ra.IsActive)
            .Include(ra => ra.Apartment)
            .ToListAsync();

        return ras.Select(ra => new ApartmentWithType
        {
            ApartmentId = ra.ApartmentId,
            Display = $"Căn {ra.Apartment!.ApartmentNumber}" +
                (string.IsNullOrWhiteSpace(ra.Apartment.BuildingBlock)
                    ? ""
                    : $", Block {ra.Apartment.BuildingBlock}") +
                $" — {(ra.ResidencyType == ResidencyType.Owner ? "Chủ sở hữu" : "Người thuê")}",
            ResidencyType = ra.ResidencyType
        }).ToList();
    }

    public async Task<CardStatsResult> GetStatsAsync()
    {
        var allCards = await _residentCardRepository.GetAllAsync();
        return new CardStatsResult
        {
            Total = allCards.Count(),
            Active = allCards.Count(c => c.Status == CardStatus.Active),
            Locked = allCards.Count(c => c.Status == CardStatus.Locked),
            Lost = allCards.Count(c => c.Status == CardStatus.Lost),
            Expired = allCards.Count(c => c.Status == CardStatus.Expired)
        };
    }
}
