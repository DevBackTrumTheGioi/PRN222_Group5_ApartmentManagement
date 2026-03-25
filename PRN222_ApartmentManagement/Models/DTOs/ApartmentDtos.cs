using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models.DTOs;

public class CreateApartmentDto
{
    public string ApartmentNumber { get; set; } = "";
    public int Floor { get; set; }
    public string? BuildingBlock { get; set; }
    public decimal? Area { get; set; }
    public string? ApartmentType { get; set; }
    public string? Description { get; set; }
}

public class UpdateApartmentDto
{
    public string ApartmentNumber { get; set; } = "";
    public int Floor { get; set; }
    public string? BuildingBlock { get; set; }
    public decimal? Area { get; set; }
    public string? ApartmentType { get; set; }
    public string? Description { get; set; }
}

public class ApartmentDetailDto
{
    public Apartment Apartment { get; set; } = null!;

    public List<ResidentHistoryItem> ResidentHistory { get; set; } = new();

    public List<ResidentCard> ResidentCards { get; set; } = new();

    public ApartmentCardStats CardStats { get; set; } = new();

    public int TotalResidents { get; set; }
}

public class ResidentHistoryItem
{
    public int UserId { get; set; }
    public string FullName { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public ResidencyType ResidencyType { get; set; }
    public string ResidencyTypeName => ResidencyType switch
    {
        ResidencyType.Owner => "Chủ sở hữu",
        ResidencyType.Tenant => "Người thuê",
        ResidencyType.FamilyMember => "Thành viên gia đình",
        _ => "Khác"
    };
    public string GetTypeBgCss => ResidencyType switch
    {
        ResidencyType.Owner => "bg-violet-50 text-violet-700 ring-1 ring-violet-200",
        ResidencyType.Tenant => "bg-blue-50 text-blue-700 ring-1 ring-blue-200",
        ResidencyType.FamilyMember => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
        _ => "bg-slate-100 text-slate-600 ring-1 ring-slate-200"
    };
    public DateTime? MoveInDate { get; set; }
    public DateTime? MoveOutDate { get; set; }
    public bool IsActive { get; set; }
    public string? ContractNumber { get; set; }
}

public class ApartmentCardStats
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Locked { get; set; }
    public int Lost { get; set; }
    public int Expired { get; set; }
}

public class ApartmentStatsResult
{
    public int Total { get; set; }
    public int Available { get; set; }
    public int Occupied { get; set; }
    public int Reserved { get; set; }
}
