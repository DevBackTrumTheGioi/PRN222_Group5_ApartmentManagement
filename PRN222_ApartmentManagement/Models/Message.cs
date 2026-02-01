using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Messages")]
public class Message
{
    [Key]
    public int MessageId { get; set; }

    [Required]
    [ForeignKey("Conversation")]
    public int ConversationId { get; set; }

    [Required]
    [ForeignKey("Sender")]
    public int SenderId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? MessageType { get; set; }

    [MaxLength(500)]
    public string? AttachmentUrl { get; set; }

    public bool IsEdited { get; set; } = false;

    public DateTime? EditedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Conversation Conversation { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
    public virtual ICollection<MessageReadReceipt> MessageReadReceipts { get; set; } = new List<MessageReadReceipt>();
    public virtual ICollection<MessageReaction> MessageReactions { get; set; } = new List<MessageReaction>();
}

