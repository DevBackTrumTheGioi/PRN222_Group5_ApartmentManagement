using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("MeterReadings")]
public class MeterReading
{
    [Key]
    public int MeterReadingId { get; set; }

    [Required]
    [ForeignKey("Apartment")]
    public int ApartmentId { get; set; }

    [Required]
    [ForeignKey("ServiceType")]
    public int ServiceTypeId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PreviousReading { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentReading { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime ReadingDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Consumption { get; set; }

    [ForeignKey("Staff")]
    public int? StaffId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Apartment Apartment { get; set; } = null!;
    public virtual ServiceType ServiceType { get; set; } = null!;
    public virtual User? Staff { get; set; }
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
}

