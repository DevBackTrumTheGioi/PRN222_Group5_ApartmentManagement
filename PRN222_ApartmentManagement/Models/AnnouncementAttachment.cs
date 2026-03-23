using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("AnnouncementAttachments")]
public class AnnouncementAttachment
{
    [Key]
    public int AttachmentId { get; set; }

    [Required]
    [ForeignKey(nameof(Announcement))]
    public int AnnouncementId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    public long? FileSize { get; set; }

    [MaxLength(100)]
    public string? ContentType { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public virtual Announcement Announcement { get; set; } = null!;
}
