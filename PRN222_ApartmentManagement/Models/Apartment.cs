using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

/// <summary>
/// Apartment entity representing apartment units in the building
/// </summary>
[Table("Apartments")]
public class Apartment
{
    [Key]
    public int ApartmentId { get; set; }

    [Required]
    [MaxLength(20)]
    public string ApartmentNumber { get; set; } = string.Empty;

    [Required]
    public int Floor { get; set; }

    [MaxLength(10)]
    public string? BuildingBlock { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Area { get; set; }

    [MaxLength(50)]
    public string? ApartmentType { get; set; }

    public ApartmentStatus Status { get; set; } = ApartmentStatus.Available;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<User> Residents { get; set; } = new List<User>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    public virtual ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();
    public virtual ICollection<AmenityBooking> AmenityBookings { get; set; } = new List<AmenityBooking>();
    public virtual ICollection<ApartmentService> ApartmentServices { get; set; } = new List<ApartmentService>();
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public virtual ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
}
