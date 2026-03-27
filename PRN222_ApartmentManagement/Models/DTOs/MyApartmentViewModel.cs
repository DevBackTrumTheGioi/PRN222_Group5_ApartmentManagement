using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models.DTOs;

public class MyApartmentViewModel
{
    public int ApartmentId { get; set; }
    public string ApartmentNumber { get; set; } = "";
    public int Floor { get; set; }
    public string? BuildingBlock { get; set; }
    public decimal? Area { get; set; }
    public string? ApartmentType { get; set; }
    public ApartmentStatus Status { get; set; }
    public int ContractId { get; set; }
    public string? ContractNumber { get; set; }
    public ContractType? ContractType { get; set; }
    public ResidencyType ResidencyType { get; set; }
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
    public int FamilyMemberCount { get; set; }
    public int ResidentApartmentId { get; set; }
}

public class ApartmentDetailViewModel
{
    public int ApartmentId { get; set; }
    public string ApartmentNumber { get; set; } = "";
    public int Floor { get; set; }
    public string? BuildingBlock { get; set; }
    public decimal? Area { get; set; }
    public string? ApartmentType { get; set; }
    public ApartmentStatus Status { get; set; }

    public int ContractId { get; set; }
    public string? ContractNumber { get; set; }
    public ContractType? ContractType { get; set; }
    public ResidencyType ResidencyType { get; set; }
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? PurchasePrice { get; set; }
}

public class ApartmentResidentDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? IdentityCardNumber { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public ResidencyType ResidencyType { get; set; }
    public string ResidencyTypeName => ResidencyType switch
    {
        ResidencyType.Owner => "Chủ sở hữu",
        ResidencyType.Tenant => "Người thuê",
        ResidencyType.FamilyMember => "Thành viên gia đình",
        _ => "Khác"
    };
}
