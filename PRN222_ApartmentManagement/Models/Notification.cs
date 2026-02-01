using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Notifications")]
public class Notification
{
    [Key]
    public int NotificationId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? NotificationType { get; set; }

    [MaxLength(50)]
    public string? ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime? ReadAt { get; set; }

    public int Priority { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual User User { get; set; } = null!;
}

