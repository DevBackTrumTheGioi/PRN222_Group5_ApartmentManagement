using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public UserRole? MemberRole { get; set; }

    [MaxLength(20)]
    public string SignatureStatus { get; set; } = "Pending";

    [Column(TypeName = "date")]
    public DateTime? SignedDate { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Contract Contract { get; set; } = null!;
    public virtual User Resident { get; set; } = null!;
}
