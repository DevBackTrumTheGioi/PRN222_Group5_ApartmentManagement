using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Parcels")]
public class Parcel
{
    [Key]
    public int ParcelId { get; set; }

    [MaxLength(100)]
    public string? TrackingNumber { get; set; }

    [Required]
    [ForeignKey("Apartment")]
    public int ApartmentId { get; set; }

    [MaxLength(200)]
    public string? RecipientName { get; set; }

    [MaxLength(200)]
    public string? Sender { get; set; }

    [Required]
    public DateTime ReceivedDate { get; set; }

    [Required]
    [ForeignKey("ReceivedByUser")]
    public int ReceivedBy { get; set; }

    public bool NotificationSent { get; set; } = false;

    public DateTime? PickedUpDate { get; set; }

    [ForeignKey("PickedUpByResident")]
    public int? PickedUpBy { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Received";

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Apartment Apartment { get; set; } = null!;
    public virtual User ReceivedByUser { get; set; } = null!;
    public virtual Resident? PickedUpByResident { get; set; }
}

