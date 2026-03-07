using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

[Table("Announcements")]
public class Announcement
{
    [Key]
    public int AnnouncementId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public AnnouncementType? AnnouncementType { get; set; }

    public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;

    [Required]
    public DateTime PublishedDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    [MaxLength(20)]
    public string Source { get; set; } = "BQL";  // "BQL" | "BQT" | "System"

    [Required]
    [ForeignKey("Creator")]
    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual User Creator { get; set; } = null!;
}
