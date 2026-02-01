using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("ConversationParticipants")]
public class ConversationParticipant
{
    [Key]
    public int ParticipantId { get; set; }

    [Required]
    [ForeignKey("Conversation")]
    public int ConversationId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.Now;

    public DateTime? LeftAt { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(50)]
    public string? Role { get; set; }

    public DateTime? LastReadAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

