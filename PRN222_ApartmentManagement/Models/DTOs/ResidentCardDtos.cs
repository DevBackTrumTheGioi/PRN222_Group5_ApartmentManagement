using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models.DTOs;

public class CreateResidentCardDto
{
    public int ResidentId { get; set; }
    public int? ApartmentId { get; set; }
    public CardType? CardType { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateResidentCardDto
{
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
}

public class UserSearchResult
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public bool IsStaff { get; set; }
    public bool HasApartments { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}

public class ApartmentWithType
{
    public int ApartmentId { get; set; }
    public string Display { get; set; } = "";
    public ResidencyType ResidencyType { get; set; }
}
