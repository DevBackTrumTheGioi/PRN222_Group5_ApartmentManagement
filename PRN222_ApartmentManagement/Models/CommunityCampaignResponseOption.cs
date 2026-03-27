using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("CommunityCampaignResponseOptions")]
public class CommunityCampaignResponseOption
{
    [Key]
    public int ResponseOptionId { get; set; }

    [Required]
    [ForeignKey("Response")]
    public int ResponseId { get; set; }

    [Required]
    [ForeignKey("Option")]
    public int OptionId { get; set; }

    public virtual CommunityCampaignResponse Response { get; set; } = null!;
    public virtual CommunityCampaignOption Option { get; set; } = null!;
}
