using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

[Table("CommunityCampaigns")]
public class CommunityCampaign
{
    [Key]
    public int CampaignId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(500)]
    public string QuestionText { get; set; } = string.Empty;

    public CommunityCampaignType CampaignType { get; set; }

    public CommunityCampaignStatus Status { get; set; } = CommunityCampaignStatus.Published;

    public bool AllowMultipleChoices { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    [Required]
    [ForeignKey("Creator")]
    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual User Creator { get; set; } = null!;
    public virtual ICollection<CommunityCampaignOption> Options { get; set; } = new List<CommunityCampaignOption>();
    public virtual ICollection<CommunityCampaignResponse> Responses { get; set; } = new List<CommunityCampaignResponse>();
}
