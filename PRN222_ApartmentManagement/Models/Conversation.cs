using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Conversations")]
public class Conversation
{
    [Key]
    public int ConversationId { get; set; }

    [MaxLength(200)]
    public string? ConversationName { get; set; }

    [MaxLength(50)]
    public string? ConversationType { get; set; }

    [Required]
    [ForeignKey("Creator")]
    public int CreatedBy { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastMessageAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual User Creator { get; set; } = null!;
    public virtual ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}

