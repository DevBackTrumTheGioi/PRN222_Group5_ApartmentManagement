using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class ApartmentService : IApartmentService
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IResidentCardRepository _residentCardRepository;
    private readonly IResidentApartmentRepository _residentApartmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IActivityLogService _activityLogService;
    private readonly ApartmentDbContext _dbContext;

    public ApartmentService(
        IApartmentRepository apartmentRepository,
        IResidentCardRepository residentCardRepository,
        IResidentApartmentRepository residentApartmentRepository,
        IUserRepository userRepository,
        IActivityLogService activityLogService,
        ApartmentDbContext dbContext)
    {
        _apartmentRepository = apartmentRepository;
        _residentCardRepository = residentCardRepository;
        _residentApartmentRepository = residentApartmentRepository;
        _userRepository = userRepository;
        _activityLogService = activityLogService;
        _dbContext = dbContext;
    }

    public async Task<PagedResult<Apartment>> GetPagedAsync(
        string? searchTerm,
        ApartmentStatus? status,
        string? type,
        string? block,
        int? floor,
        int pageIndex,
        int pageSize)
    {
        var query = _dbContext.Apartments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(a =>
                a.ApartmentNumber.ToLower().Contains(term) ||
                (a.BuildingBlock != null && a.BuildingBlock.ToLower().Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(a => a.ApartmentType == type);
        }

        if (!string.IsNullOrWhiteSpace(block))
        {
            query = query.Where(a => a.BuildingBlock == block);
        }

        if (floor.HasValue && floor.Value > 0)
        {
            query = query.Where(a => a.Floor == floor.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(a => a.BuildingBlock)
            .ThenBy(a => a.Floor)
            .ThenBy(a => a.ApartmentNumber)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Apartment>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<ApartmentDetailDto?> GetDetailAsync(int id)
    {
        var apt = await _apartmentRepository.GetByIdAsync(id);
        if (apt == null) return null;

        // Resident history from ResidentApartment join table (includes current + past residents)
        var history = await _dbContext.ResidentApartments
            .Where(ra => ra.ApartmentId == id)
            .Include(ra => ra.User)
            .Include(ra => ra.Contract)
            .OrderByDescending(ra => ra.IsActive)
            .ThenByDescending(ra => ra.MoveInDate)
            .ToListAsync();

        // Resident cards linked to this apartment
        var cards = await _dbContext.ResidentCards
            .Where(rc => rc.ApartmentId == id)
            .Include(rc => rc.Resident)
            .OrderByDescending(rc => rc.CreatedAt)
            .ToListAsync();

        // Card stats
        var cardStats = new ApartmentCardStats
        {
            Total = cards.Count,
            Active = cards.Count(c => c.Status == CardStatus.Active && !c.IsDeleted),
            Locked = cards.Count(c => c.Status == CardStatus.Locked),
            Lost = cards.Count(c => c.Status == CardStatus.Lost),
            Expired = cards.Count(c => c.Status == CardStatus.Expired)
        };

        var totalResidents = history.Select(h => h.UserId).Distinct().Count();

        return new ApartmentDetailDto
        {
            Apartment = apt,
            ResidentHistory = history.Select(h => new ResidentHistoryItem
            {
                UserId = h.UserId,
                FullName = h.User?.FullName ?? "N/A",
                PhoneNumber = h.User?.PhoneNumber,
                ResidencyType = h.ResidencyType,
                MoveInDate = h.MoveInDate,
                MoveOutDate = h.MoveOutDate,
                IsActive = h.IsActive,
                ContractNumber = h.Contract?.ContractNumber
            }).ToList(),
            ResidentCards = cards,
            CardStats = cardStats,
            TotalResidents = totalResidents
        };
    }

    public async Task<(bool Success, string Message)> CreateAsync(CreateApartmentDto dto, int creatorId)
    {
        if (string.IsNullOrWhiteSpace(dto.ApartmentNumber))
            return (false, "Số căn hộ không được để trống.");

        if (dto.Floor <= 0)
            return (false, "Tầng phải lớn hơn 0.");

        var exists = await _dbContext.Apartments
            .AnyAsync(a => a.ApartmentNumber == dto.ApartmentNumber.Trim());
        if (exists)
            return (false, $"Căn hộ '{dto.ApartmentNumber}' đã tồn tại.");

        var apt = new Apartment
        {
            ApartmentNumber = dto.ApartmentNumber.Trim(),
            Floor = dto.Floor,
            BuildingBlock = dto.BuildingBlock?.Trim(),
            Area = dto.Area,
            ApartmentType = dto.ApartmentType?.Trim(),
            Status = ApartmentStatus.Available,
            Description = dto.Description?.Trim(),
            CreatedAt = DateTime.Now
        };

        await _apartmentRepository.AddAsync(apt);
        return (true, $"Tạo căn hộ '{apt.ApartmentNumber}' thành công.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(int id, UpdateApartmentDto dto)
    {
        var apt = await _apartmentRepository.GetByIdAsync(id);
        if (apt == null)
            return (false, "Không tìm thấy căn hộ.");

        if (string.IsNullOrWhiteSpace(dto.ApartmentNumber))
            return (false, "Số căn hộ không được để trống.");

        if (dto.Floor <= 0)
            return (false, "Tầng phải lớn hơn 0.");

        var duplicate = await _dbContext.Apartments
            .AnyAsync(a => a.ApartmentId != id && a.ApartmentNumber == dto.ApartmentNumber.Trim());
        if (duplicate)
            return (false, $"Số căn hộ '{dto.ApartmentNumber}' đã được sử dụng bởi căn khác.");

        apt.ApartmentNumber = dto.ApartmentNumber.Trim();
        apt.Floor = dto.Floor;
        apt.BuildingBlock = dto.BuildingBlock?.Trim();
        apt.Area = dto.Area;
        apt.ApartmentType = dto.ApartmentType?.Trim();
        apt.Description = dto.Description?.Trim();
        apt.UpdatedAt = DateTime.Now;

        await _apartmentRepository.UpdateAsync(apt);
        return (true, $"Cập nhật căn hộ '{apt.ApartmentNumber}' thành công.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var apt = await _apartmentRepository.GetByIdAsync(id);
        if (apt == null)
            return (false, "Không tìm thấy căn hộ.");

        // Check for active residents
        var hasActiveResidents = await _dbContext.ResidentApartments
            .AnyAsync(ra => ra.ApartmentId == id && ra.IsActive);
        if (hasActiveResidents)
            return (false, "Không thể xóa căn hộ đang có cư dân cư trú.");

        // Check for active cards
        var hasActiveCards = await _dbContext.ResidentCards
            .AnyAsync(rc => rc.ApartmentId == id && !rc.IsDeleted);
        if (hasActiveCards)
            return (false, "Không thể xóa căn hộ đang có thẻ đang hoạt động.");

        await _apartmentRepository.DeleteAsync(id);
        return (true, $"Xóa căn hộ '{apt.ApartmentNumber}' thành công.");
    }

    public async Task<ApartmentStatsResult> GetStatsAsync()
    {
        var all = await _apartmentRepository.GetAllAsync();
        var list = all.ToList();
        return new ApartmentStatsResult
        {
            Total = list.Count,
            Available = list.Count(a => a.Status == ApartmentStatus.Available),
            Occupied = list.Count(a => a.Status == ApartmentStatus.Occupied),
            Reserved = list.Count(a => a.Status == ApartmentStatus.Reserved)
        };
    }

    public async Task<List<Apartment>> GetAllAsync()
    {
        var result = await _apartmentRepository.GetAllAsync();
        return result.ToList();
    }
}
