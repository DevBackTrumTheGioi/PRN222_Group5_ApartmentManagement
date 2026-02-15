using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

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

    public NotificationType NotificationType { get; set; } = NotificationType.Other;

    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;

    public int? ReferenceId { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime? ReadAt { get; set; }

    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual User User { get; set; } = null!;
}
