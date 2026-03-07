using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

[Table("Requests")]
public class Request
{
  [Key]
  public int RequestId { get; set; }

  [Required]
  [MaxLength(50)]
  public string RequestNumber { get; set; } = string.Empty;

  [Required]
  [ForeignKey("Apartment")]
  public int ApartmentId { get; set; }

  [Required]
  [ForeignKey("Resident")]
  public int ResidentId { get; set; }

  public RequestType? RequestType { get; set; }

  [Required]
  [MaxLength(200)]
  public string Title { get; set; } = string.Empty;

  public string? Description { get; set; }

  public RequestPriority Priority { get; set; } = RequestPriority.Normal;

  public RequestStatus Status { get; set; } = RequestStatus.Pending;

  [ForeignKey("AssignedUser")]
  public int? AssignedTo { get; set; }

  [ForeignKey("EscalatedToUser")]
  public int? EscalatedTo { get; set; }

  public DateTime? EscalatedAt { get; set; }

  [MaxLength(500)]
  public string? EscalationReason { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.Now;

  public DateTime? UpdatedAt { get; set; }

  public DateTime? ResolvedAt { get; set; }

  public virtual Apartment Apartment { get; set; } = null!;
  public virtual User Resident { get; set; } = null!;
  public virtual User? AssignedUser { get; set; }
  public virtual User? EscalatedToUser { get; set; }
  public virtual ICollection<RequestAttachment> RequestAttachments { get; set; } = new List<RequestAttachment>();
}
