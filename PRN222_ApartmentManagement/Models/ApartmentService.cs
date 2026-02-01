using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("ApartmentServices")]
public class ApartmentService
{
    [Key]
    public int ApartmentServiceId { get; set; }

    [Required]
    [ForeignKey("Apartment")]
    public int ApartmentId { get; set; }

    [Required]
    [ForeignKey("ServiceType")]
    public int ServiceTypeId { get; set; }

    public int Quantity { get; set; } = 1;

    [Required]
    [Column(TypeName = "date")]
    public DateTime RegisteredFrom { get; set; }

    [Column(TypeName = "date")]
    public DateTime? RegisteredTo { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Apartment Apartment { get; set; } = null!;
    public virtual ServiceType ServiceType { get; set; } = null!;
}

