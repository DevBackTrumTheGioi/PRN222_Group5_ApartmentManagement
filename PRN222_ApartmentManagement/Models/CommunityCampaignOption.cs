using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("CommunityCampaignOptions")]
public class CommunityCampaignOption
{
    [Key]
    public int OptionId { get; set; }

    [Required]
    [ForeignKey("Campaign")]
    public int CampaignId { get; set; }

    [Required]
    [MaxLength(300)]
    public string OptionText { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public virtual CommunityCampaign Campaign { get; set; } = null!;
    public virtual ICollection<CommunityCampaignResponseOption> ResponseSelections { get; set; } = new List<CommunityCampaignResponseOption>();
}
