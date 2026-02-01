using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [MaxLength(50)]
    public string? AnnouncementType { get; set; }

    public int Priority { get; set; } = 1;

    [Required]
    public DateTime PublishedDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    [ForeignKey("Creator")]
    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual User Creator { get; set; } = null!;
}

