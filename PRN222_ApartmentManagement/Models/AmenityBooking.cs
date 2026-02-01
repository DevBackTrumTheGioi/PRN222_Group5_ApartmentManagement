using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("AmenityBookings")]
public class AmenityBooking
{
    [Key]
    public int BookingId { get; set; }

    [Required]
    [ForeignKey("Amenity")]
    public int AmenityId { get; set; }

    [Required]
    [ForeignKey("Apartment")]
    public int ApartmentId { get; set; }

    [Required]
    [ForeignKey("Resident")]
    public int ResidentId { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime BookingDate { get; set; }

    [Required]
    [Column(TypeName = "time")]
    public TimeSpan StartTime { get; set; }

    [Required]
    [Column(TypeName = "time")]
    public TimeSpan EndTime { get; set; }

    [Column(TypeName = "decimal(4,2)")]
    public decimal? TotalHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual Amenity Amenity { get; set; } = null!;
    public virtual Apartment Apartment { get; set; } = null!;
    public virtual Resident Resident { get; set; } = null!;
}

