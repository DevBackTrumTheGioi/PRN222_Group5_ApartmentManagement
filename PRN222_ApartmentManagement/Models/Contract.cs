using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

[Table("Contracts")]
public class Contract
{
    [Key]
    public int ContractId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ContractNumber { get; set; } = string.Empty;

    [Required]
    [ForeignKey("Apartment")]
    public int ApartmentId { get; set; }

    public ContractType? ContractType { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MonthlyRent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DepositAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PurchasePrice { get; set; }

    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    public string? Terms { get; set; }

    [MaxLength(500)]
    public string? ContractFile { get; set; }

    [Column(TypeName = "date")]
    public DateTime? SignedDate { get; set; }

    [Required]
    [ForeignKey("Creator")]
    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? TerminatedAt { get; set; }

    [MaxLength(500)]
    public string? TerminationReason { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Thong tin chu ho (luu tru de tao tai khoan khi kich hoat hop dong)
    [MaxLength(200)]
    public string? OwnerFullName { get; set; }

    [MaxLength(100)]
    public string? OwnerEmail { get; set; }

    [MaxLength(20)]
    public string? OwnerPhone { get; set; }

    [Column(TypeName = "date")]
    public DateTime? OwnerDateOfBirth { get; set; }

    [MaxLength(20)]
    public string? OwnerIdentityCard { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;
    public virtual User Creator { get; set; } = null!;
    public virtual ICollection<ContractMember> ContractMembers { get; set; } = new List<ContractMember>();
    public virtual ICollection<ResidentApartment> ResidentApartments { get; set; } = new List<ResidentApartment>();
}
