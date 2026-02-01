using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [MaxLength(50)]
    public string? ContractType { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MonthlyRent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DepositAmount { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Draft";

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

    public virtual Apartment Apartment { get; set; } = null!;
    public virtual User Creator { get; set; } = null!;
    public virtual ICollection<ContractMember> ContractMembers { get; set; } = new List<ContractMember>();
}

