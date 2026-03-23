using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Users")]
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public UserRole? Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLogin { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; } = false;

    // Resident specific fields
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? IdentityCardNumber { get; set; }

    public ResidentType? ResidentType { get; set; }

    [MaxLength(20)]
    public string? ResidencyStatus { get; set; }

    [ForeignKey("Apartment")]
    public int? ApartmentId { get; set; }

    public DateTime? MoveInDate { get; set; }

    public DateTime? MoveOutDate { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    /// <summary>
    /// Chuỗi JSON chứa face descriptor (vector 128 số) từ face-api.js
    /// </summary>
    public string? FaceDescriptor { get; set; }

    public bool IsFaceRegistered { get; set; } = false;

    // Navigation properties
    public virtual ICollection<Invoice> CreatedInvoices { get; set; } = new List<Invoice>();
    public virtual ICollection<Request> AssignedRequests { get; set; } = new List<Request>();
    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Contract> CreatedContracts { get; set; } = new List<Contract>();

    // Resident navigation properties
    public virtual Apartment? Apartment { get; set; }
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public virtual ICollection<ResidentCard> ResidentCards { get; set; } = new List<ResidentCard>();
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    public virtual ICollection<Visitor> RegisteredVisitors { get; set; } = new List<Visitor>();
    public virtual ICollection<AmenityBooking> AmenityBookings { get; set; } = new List<AmenityBooking>();
    public virtual ICollection<ContractMember> ContractMembers { get; set; } = new List<ContractMember>();
    public virtual ICollection<ResidentApartment> ResidentApartments { get; set; } = new List<ResidentApartment>();
    public virtual ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    public virtual ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();

    // ServiceOrder navigation properties
    public virtual ICollection<ServiceOrder> AssignedServiceOrders { get; set; } = new List<ServiceOrder>();
    public virtual ICollection<ServiceOrder> CompletedServiceOrders { get; set; } = new List<ServiceOrder>();
}
