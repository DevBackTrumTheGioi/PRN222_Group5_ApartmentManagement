using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [MaxLength(50)]
    public string? RequestType { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Priority { get; set; } = 1;

    public int Status { get; set; } = 0;

    [ForeignKey("AssignedUser")]
    public int? AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;
    public virtual Resident Resident { get; set; } = null!;
    public virtual User? AssignedUser { get; set; }
    public virtual ICollection<RequestAttachment> RequestAttachments { get; set; } = new List<RequestAttachment>();
}

