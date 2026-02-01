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

    [MaxLength(50)]
    public string? Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLogin { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Invoice> CreatedInvoices { get; set; } = new List<Invoice>();
    public virtual ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
    public virtual ICollection<Request> AssignedRequests { get; set; } = new List<Request>();
    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    public virtual ICollection<Parcel> ReceivedParcels { get; set; } = new List<Parcel>();
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Conversation> CreatedConversations { get; set; } = new List<Conversation>();
    public virtual ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<MessageReadReceipt> MessageReadReceipts { get; set; } = new List<MessageReadReceipt>();
    public virtual ICollection<MessageReaction> MessageReactions { get; set; } = new List<MessageReaction>();
    public virtual ICollection<Contract> CreatedContracts { get; set; } = new List<Contract>();
}

