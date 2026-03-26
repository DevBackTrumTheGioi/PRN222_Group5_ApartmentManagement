using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

[Table("Meetings")]
public class Meeting
{
    [Key]
    public int MeetingId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public MeetingType MeetingType { get; set; } = MeetingType.BQT;

    [MaxLength(200)]
    public string? Location { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public MeetingStatus Status { get; set; } = MeetingStatus.Scheduled;

    [Required]
    [ForeignKey("Creator")]
    public int CreatedBy { get; set; }

    public string? MinutesContent { get; set; }

    [MaxLength(200)]
    public string? MinutesFileName { get; set; }

    [MaxLength(500)]
    public string? MinutesFilePath { get; set; }

    public string? Attendees { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual User Creator { get; set; } = null!;
}
