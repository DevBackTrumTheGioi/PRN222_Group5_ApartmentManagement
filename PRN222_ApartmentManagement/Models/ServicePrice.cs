using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("ServicePrices")]
public class ServicePrice
{
    [Key]
    public int ServicePriceId { get; set; }

    [Required]
    [ForeignKey("ServiceType")]
    public int ServiceTypeId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime EffectiveFrom { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EffectiveTo { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual ServiceType ServiceType { get; set; } = null!;
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
}

