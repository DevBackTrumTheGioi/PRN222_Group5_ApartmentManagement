using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("AnnouncementReads")]
public class AnnouncementRead
{
    [Key]
    public int AnnouncementReadId { get; set; }

    [Required]
    [ForeignKey(nameof(Announcement))]
    public int AnnouncementId { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public DateTime ReadAt { get; set; } = DateTime.Now;

    public virtual Announcement Announcement { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
