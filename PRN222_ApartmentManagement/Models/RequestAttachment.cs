using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("RequestAttachments")]
public class RequestAttachment
{
    [Key]
    public int AttachmentId { get; set; }

    [Required]
    [ForeignKey("Request")]
    public int RequestId { get; set; }

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

    public virtual Request Request { get; set; } = null!;
}

