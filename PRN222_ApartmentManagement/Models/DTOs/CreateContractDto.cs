using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models.DTOs;

public class CreateContractDto
{
    public int ApartmentId { get; set; }
    public ContractType? ContractType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Terms { get; set; }

    // Chủ hộ mới (nullable để không bị implicit [Required] khi chọn chủ cũ)
    public string? OwnerFullName { get; set; }
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
    public DateTime? OwnerDateOfBirth { get; set; }
    public string? OwnerIdentityCard { get; set; }

    // Chủ cũ (tìm theo CCCD)
    public bool IsExistingOwner { get; set; }
    public int? ExistingOwnerId { get; set; }
    public string? ExistingOwnerCccd { get; set; }
}
