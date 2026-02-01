using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("MessageReactions")]
public class MessageReaction
{
    [Key]
    public int ReactionId { get; set; }

    [Required]
    [ForeignKey("Message")]
    public int MessageId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [MaxLength(50)]
    public string? ReactionType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Message Message { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

