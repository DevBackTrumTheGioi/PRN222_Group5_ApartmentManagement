using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

/// <summary>
/// Ghi nhận lịch sử cư trú của User tại Apartment.
/// Một User có thể có nhiều bản ghi ResidentApartment (nhiều căn / nhiều giai đoạn).
/// </summary>
[Table("ResidentApartments")]
public class ResidentApartment
{
    [Key]
    public int ResidentApartmentId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ApartmentId { get; set; }

    [Required]
    public int ContractId { get; set; }

    public ResidencyType ResidencyType { get; set; }

    /// <summary>
    /// Đang cư trú tại căn này hay đã chuyển đi.
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime? MoveInDate { get; set; }

    public DateTime? MoveOutDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [ForeignKey("ApartmentId")]
    public virtual Apartment? Apartment { get; set; }

    [ForeignKey("ContractId")]
    public virtual Contract? Contract { get; set; }
}
