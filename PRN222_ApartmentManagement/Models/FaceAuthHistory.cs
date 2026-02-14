using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("FaceAuthHistory")]
public class FaceAuthHistory
{
    [Key]
    public int HistoryId { get; set; }

    [Required]
    public int ResidentId { get; set; }

    [ForeignKey("ResidentId")]
    public virtual Resident Resident { get; set; } = null!;

    public DateTime AuthTime { get; set; } = DateTime.Now;

    public bool IsSuccess { get; set; }

    public double ConfidenceScore { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(200)]
    public string? DeviceInfo { get; set; }
}

