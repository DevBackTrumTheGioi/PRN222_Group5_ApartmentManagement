using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Amenities")]
public class Amenity
{
    [Key]
    public int AmenityId { get; set; }

    [Required]
    [MaxLength(100)]
    public string AmenityName { get; set; } = string.Empty;

    public int? AmenityTypeId { get; set; }

    [ForeignKey("AmenityTypeId")]
    public virtual AmenityType? AmenityType { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    public int? Capacity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PricePerHour { get; set; }

    public bool RequiresBooking { get; set; } = true;

    [Column(TypeName = "time")]
    public TimeSpan OpenTime { get; set; } = new(6, 0, 0);

    [Column(TypeName = "time")]
    public TimeSpan CloseTime { get; set; } = new(22, 0, 0);

    public int CancellationDeadlineHours { get; set; } = 4;

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<AmenityBooking> AmenityBookings { get; set; } = new List<AmenityBooking>();
}
