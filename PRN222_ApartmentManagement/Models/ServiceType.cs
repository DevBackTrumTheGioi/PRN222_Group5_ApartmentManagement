using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("ServiceTypes")]
public class ServiceType
{
    [Key]
    public int ServiceTypeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ServiceTypeName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? MeasurementUnit { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = new List<ServicePrice>();
    public virtual ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    public virtual ICollection<ApartmentService> ApartmentServices { get; set; } = new List<ApartmentService>();
}

