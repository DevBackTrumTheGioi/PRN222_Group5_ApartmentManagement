using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

/// <summary>
/// ServiceOrder entity representing on-demand service requests from residents
/// (e.g., laundry, cleaning, minor repairs)
/// </summary>
[Table("ServiceOrders")]
public class ServiceOrder
{
    [Key]
    public int ServiceOrderId { get; set; }

    /// <summary>
    /// Order number format: SO-YYYYMMDD-XXX
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    [ForeignKey("Apartment")]
    public int ApartmentId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int ResidentId { get; set; }

    [Required]
    [ForeignKey("ServiceType")]
    public int ServiceTypeId { get; set; }

    // Booking information
    [Required]
    [Column(TypeName = "date")]
    public DateTime RequestedDate { get; set; }

    /// <summary>
    /// Time slot: 'Morning', 'Afternoon', 'Evening'
    /// </summary>
    [MaxLength(50)]
    public string? RequestedTimeSlot { get; set; }

    /// <summary>
    /// Detailed description of the service request
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    // Status & assignment
    /// <summary>
    /// 0: Pending, 1: Confirmed, 2: InProgress, 3: Completed, 4: Cancelled
    /// </summary>
    public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Pending;

    [ForeignKey("AssignedStaff")]
    public int? AssignedTo { get; set; }

    public DateTime? AssignedAt { get; set; }

    // Pricing
    [Column(TypeName = "decimal(18,2)")]
    public decimal? EstimatedPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ActualPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AdditionalCharges { get; set; } = 0;

    [MaxLength(500)]
    public string? ChargeNotes { get; set; }

    // Completion
    public DateTime? CompletedAt { get; set; }

    [ForeignKey("CompletedByUser")]
    public int? CompletedBy { get; set; }

    [MaxLength(2000)]
    public string? CompletionNotes { get; set; }

    // Rating & Review (1-5 stars)
    [Range(1, 5)]
    public int? Rating { get; set; }

    [MaxLength(500)]
    public string? ReviewComment { get; set; }

    public DateTime? ReviewedAt { get; set; }

    // Invoice linkage
    [ForeignKey("Invoice")]
    public int? InvoiceId { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    [MaxLength(500)]
    public string? CancelReason { get; set; }

    // Navigation properties
    public virtual Apartment Apartment { get; set; } = null!;
    public virtual User Resident { get; set; } = null!;
    public virtual ServiceType ServiceType { get; set; } = null!;
    public virtual User? AssignedStaff { get; set; }
    public virtual User? CompletedByUser { get; set; }
    public virtual Invoice? Invoice { get; set; }
}

/// <summary>
/// Enum for ServiceOrder status
/// </summary>
public enum ServiceOrderStatus
{
    Pending = 0,
    Confirmed = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}

/// <summary>
/// Enum for requested time slots
/// </summary>
public enum TimeSlot
{
    Morning,    // 8:00 - 12:00
    Afternoon,  // 13:00 - 17:00
    Evening     // 18:00 - 21:00
}
