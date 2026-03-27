using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class ResidentApartmentAccessService : IResidentApartmentAccessService
{
    private readonly ApartmentDbContext _context;

    public ResidentApartmentAccessService(ApartmentDbContext context)
    {
        _context = context;
    }

    public Task<List<int>> GetActiveApartmentIdsAsync(int residentUserId)
    {
        return BuildActiveResidentApartmentQuery(residentUserId)
            .Select(ra => ra.ApartmentId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<ApartmentWithType>> GetActiveApartmentOptionsAsync(int residentUserId)
    {
        var residentApartments = await BuildActiveResidentApartmentQuery(residentUserId)
            .Include(ra => ra.Apartment)
            .ToListAsync();

        return residentApartments
            .GroupBy(ra => new
            {
                ra.ApartmentId,
                ra.ResidencyType,
                ra.Apartment!.ApartmentNumber,
                ra.Apartment.BuildingBlock
            })
            .OrderBy(g => GetResidencyPriority(g.Key.ResidencyType))
            .ThenBy(g => g.Key.BuildingBlock)
            .ThenBy(g => g.Key.ApartmentNumber)
            .Select(g => new ApartmentWithType
            {
                ApartmentId = g.Key.ApartmentId,
                ResidencyType = g.Key.ResidencyType,
                Display = BuildDisplayLabel(g.Key.ApartmentNumber, g.Key.BuildingBlock, g.Key.ResidencyType)
            })
            .ToList();
    }

    public Task<bool> HasAnyActiveApartmentAsync(int residentUserId)
    {
        return BuildActiveResidentApartmentQuery(residentUserId).AnyAsync();
    }

    public Task<bool> IsResidentInApartmentAsync(int residentUserId, int apartmentId)
    {
        return BuildActiveResidentApartmentQuery(residentUserId)
            .AnyAsync(ra => ra.ApartmentId == apartmentId);
    }

    public async Task<Apartment?> GetPreferredApartmentAsync(int residentUserId)
    {
        var activeApartments = await BuildActiveResidentApartmentQuery(residentUserId)
            .Select(ra => new
            {
                ra.ApartmentId,
                ra.ResidencyType,
                ra.MoveInDate
            })
            .ToListAsync();

        var preferredApartmentId = activeApartments
            .OrderBy(ra => GetResidencyPriority(ra.ResidencyType))
            .ThenByDescending(ra => ra.MoveInDate ?? DateTime.MinValue)
            .Select(ra => (int?)ra.ApartmentId)
            .FirstOrDefault();

        if (!preferredApartmentId.HasValue)
        {
            return null;
        }

        return await _context.Apartments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ApartmentId == preferredApartmentId.Value);
    }

    private IQueryable<ResidentApartment> BuildActiveResidentApartmentQuery(int residentUserId)
    {
        return _context.ResidentApartments
            .AsNoTracking()
            .Where(ra =>
                ra.UserId == residentUserId &&
                ra.IsActive &&
                ra.User != null &&
                ra.User.IsActive &&
                !ra.User.IsDeleted &&
                ra.ApartmentId > 0);
    }

    private static string BuildDisplayLabel(string apartmentNumber, string? buildingBlock, ResidencyType residencyType)
    {
        var apartmentLabel = string.IsNullOrWhiteSpace(buildingBlock)
            ? $"Căn {apartmentNumber}"
            : $"Căn {buildingBlock}-{apartmentNumber}";

        return $"{apartmentLabel} ({GetResidencyLabel(residencyType)})";
    }

    private static int GetResidencyPriority(ResidencyType residencyType)
    {
        return residencyType switch
        {
            ResidencyType.Owner => 0,
            ResidencyType.Tenant => 1,
            ResidencyType.FamilyMember => 2,
            _ => 3
        };
    }

    private static string GetResidencyLabel(ResidencyType residencyType)
    {
        return residencyType switch
        {
            ResidencyType.Owner => "Chủ hộ",
            ResidencyType.Tenant => "Người thuê",
            ResidencyType.FamilyMember => "Thành viên",
            _ => "Cư dân"
        };
    }
}
