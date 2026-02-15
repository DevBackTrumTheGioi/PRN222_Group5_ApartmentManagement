using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

[Table("ContractMembers")]
public class ContractMember
{
    [Key]
    public int ContractMemberId { get; set; }

    [Required]
    [ForeignKey("Contract")]
    public int ContractId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int ResidentId { get; set; }

    public MemberRole? MemberRole { get; set; }

    public SignatureStatus SignatureStatus { get; set; } = SignatureStatus.Pending;

    [Column(TypeName = "date")]
    public DateTime? SignedDate { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Contract Contract { get; set; } = null!;
    public virtual User Resident { get; set; } = null!;
}
