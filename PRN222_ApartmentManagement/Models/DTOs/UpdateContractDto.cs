using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models.DTOs;

public class UpdateContractDto
{
    public ContractType? ContractType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Terms { get; set; }
    public ContractStatus? Status { get; set; }

    // Chủ hộ
    public OwnerUpdateDto? Owner { get; set; }
    public bool IsExistingOwner { get; set; }
    public int? ExistingOwnerId { get; set; }
    public string? ExistingOwnerCccd { get; set; }
}

public class OwnerUpdateDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? IdentityCardNumber { get; set; }
}
