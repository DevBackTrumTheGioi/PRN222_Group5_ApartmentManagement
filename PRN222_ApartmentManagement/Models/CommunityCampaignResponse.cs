using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("CommunityCampaignResponses")]
public class CommunityCampaignResponse
{
    [Key]
    public int ResponseId { get; set; }

    [Required]
    [ForeignKey("Campaign")]
    public int CampaignId { get; set; }

    [Required]
    [ForeignKey("Respondent")]
    public int UserId { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.Now;

    public virtual CommunityCampaign Campaign { get; set; } = null!;
    public virtual User Respondent { get; set; } = null!;
    public virtual ICollection<CommunityCampaignResponseOption> SelectedOptions { get; set; } = new List<CommunityCampaignResponseOption>();
}
