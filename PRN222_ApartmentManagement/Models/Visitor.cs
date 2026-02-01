using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Visitors")]
public class Visitor
{
    [Key]
    public int VisitorId { get; set; }

    [Required]
    [MaxLength(200)]
    public string VisitorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? IdentityCard { get; set; }

    [Required]
    [ForeignKey("Apartment")]
    public int ApartmentId { get; set; }

    [Required]
    [ForeignKey("RegisteredByResident")]
    public int RegisteredBy { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime VisitDate { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    [MaxLength(255)]
    public string? QRCode { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Apartment Apartment { get; set; } = null!;
    public virtual Resident RegisteredByResident { get; set; } = null!;
}

